using System;
using System.Text;
using Sitana.Framework.Ui.Views;
using Sitana.Framework.Cs;

namespace Nom3.BosUi
{
    public class DateTimeContext
    {
        public enum Type
        {
            Hour,
            Minute,
            Day,
            Year,
            Month
        }

        class ElementContext : ISelectorContext
        {
            DateTimeContext _context;
            Type _type;

            public ElementContext(DateTimeContext context, Type type)
            {
                _context = context;
                _type = type;
            }

            void ISelectorContext.GetData(Int32 offset, StringBuilder caption, out Boolean enabled)
            {
                _context.GetData(caption, offset, _type, out enabled);
            }

            void ISelectorContext.SetCurrent(Int32 offset)
            {
                if (offset != 0)
                {
                    _context.SetCurrent(offset, _type);
                    _context.UpdateOthers(_type);
                }
            }

            public void UpdateSelection()
            {
                _shouldUpdateSelection = true;
            }

            Boolean ISelectorContext.ShouldUpdateSelection
            {
                get
                {
                    Boolean update = _shouldUpdateSelection;
                    _shouldUpdateSelection = false;
                    return update;
                }
            }

            Boolean _shouldUpdateSelection = false;
        }

        DateTime _dateTime;

        ElementContext _day;
        ElementContext _month;
        ElementContext _year;

        ElementContext _hour;
        ElementContext _minute;

        public DateTime Selected
        {
            get
            {
                return _dateTime;
            }

            set
            {
                _dateTime = value;
            }
        }

        private String[] _months;

        private Int32 _disabledDaysOfWeek = 0;

        private Pair<Int32, Int32>? _enabledYears;

        private DateTime _minDate = DateTime.MinValue;
        private DateTime _maxDate = DateTime.MaxValue;

        public DateTimeContext(String[] months)
        {
            _day = new ElementContext(this, Type.Day);
            _month = new ElementContext(this, Type.Month);
            _year = new ElementContext(this, Type.Year);

            _hour = new ElementContext(this, Type.Hour);
            _minute = new ElementContext(this, Type.Minute);

            _months = months;
        }

        public void EnableDayOfWeek(DayOfWeek dayOfWeek, Boolean enable)
        {
            if (enable)
            {
                _disabledDaysOfWeek &= ~(1 << (Int32)dayOfWeek);
            }
            else
            {
                _disabledDaysOfWeek |= (1 << (Int32)dayOfWeek);
            }
        }

        public void SetYearRange(Int32 start, Int32 end)
        {
            _enabledYears = new Pair<Int32, Int32>(start, end);
        }

        public void SetDateRange(DateTime min, DateTime max)
        {
            _minDate = min;
            _maxDate = max;
        }

        public ISelectorContext Get(Type type)
        {
            switch (type)
            {
                case Type.Day:
                    return _day;
                case Type.Month:
                    return _month;
                case Type.Year:
                    return _year;
                case Type.Hour:
                    return _hour;
            }

            return _minute;
        }

        internal void UpdateOthers(Type type)
        {
            if (type != Type.Day)
            {
                _day.UpdateSelection();
            }
            if (type != Type.Month)
            {
                _month.UpdateSelection();
            }
            if (type != Type.Year)
            {
                _year.UpdateSelection();
            }
            if (type != Type.Hour)
            {
                _hour.UpdateSelection();
            }
            if (type != Type.Minute)
            {
                _minute.UpdateSelection();
            }
        }

        internal void GetData(StringBuilder caption, int offset, Type type, out bool enabled)
        {
            caption.Clear();

            DateTime newDateTime = ComputeDateTime(offset, type);
            enabled = newDateTime >= _minDate && newDateTime <= _maxDate;

            switch (type)
            {
                case Type.Day:

                    caption.AppendFormat("{0}", newDateTime.Day);
                    Int32 dayOfWeek = (Int32)newDateTime.DayOfWeek;

                    if ((_disabledDaysOfWeek & (1 << dayOfWeek)) != 0)
                    {
                        enabled = false;
                    }

                    break;

                case Type.Month:
                    caption.Append(_months[newDateTime.Month - 1]);
                    break;

                case Type.Year:
                    caption.AppendFormat("{0}", newDateTime.Year);

                    if (_enabledYears.HasValue)
                    {
                        if (newDateTime.Year < _enabledYears.Value.First || newDateTime.Year > _enabledYears.Value.Second)
                        {
                            enabled = false;
                        }
                    }

                    break;

                case Type.Hour:
                    caption.AppendFormat("{0:00}", newDateTime.Hour);
                    break;

                case Type.Minute:
                    caption.AppendFormat("{0:00}", newDateTime.Minute);
                    break;
            }
        }

        private DateTime ComputeDateTime(Int32 offset, Type type)
        {
            DateTime newDateTime = _dateTime;

            switch (type)
            {
                case Type.Day:
                    {
                        Int32 days = DateTime.DaysInMonth(_dateTime.Year, _dateTime.Month);
                        Int32 day = (_dateTime.Day - 1) + offset;

                        while (day < 0)
                        {
                            day += days;
                        }

                        day %= days;
                        day += 1;

                        newDateTime = new DateTime(_dateTime.Year, _dateTime.Month, day);
                    }

                    break;

                case Type.Month:
                    {
                        Int32 month = _dateTime.Month - 1;
                        month += offset;

                        while (month < 0)
                        {
                            month += 12;
                        }

                        month %= 12;
                        month += 1;

                        Int32 day = _dateTime.Day;

                        Boolean doContinue = true;

                        while (doContinue)
                        {
                            try
                            {
                                newDateTime = new DateTime(_dateTime.Year, month, day);
                                doContinue = false;
                            }
                            catch
                            {
                                day--;
                            }
                        }
                    }


                    break;

                case Type.Year:
                    newDateTime = _dateTime.AddYears(offset);
                    break;

                case Type.Hour:
                    {
                        Int32 hour = _dateTime.Hour;
                        hour += offset;

                        while (hour < 0)
                        {
                            hour += 24;
                        }

                        hour %= 24;

                        newDateTime = new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, hour, _dateTime.Minute, 0);
                    }
                    break;

                case Type.Minute:
                    {
                        Int32 minute = _dateTime.Minute;
                        minute += offset;

                        while (minute < 0)
                        {
                            minute += 60;
                        }

                        minute %= 60;

                        newDateTime = new DateTime(_dateTime.Year, _dateTime.Month, _dateTime.Day, _dateTime.Hour, minute, 0);
                    }
                    break;
            }

            return newDateTime;
        }

        internal void SetCurrent(Int32 offset, Type type)
        {
            _dateTime = ComputeDateTime(offset, type);
        }
    }
}
