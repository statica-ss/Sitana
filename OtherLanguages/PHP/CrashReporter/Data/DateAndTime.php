<?
class DateAndTime
{
	public $year;
	public $month;
	public $day;
	public $hour;
	public $minute;
	public $second;

	public function read($reader)
	{
		$this->year = $reader->readInteger(0);
		$this->month = $reader->readInteger(1);
		$this->day = $reader->readInteger(2);
		$this->hour = $reader->readInteger(3);
		$this->minute = $reader->readInteger(4);
		$this->second = $reader->readInteger(5);
	}

	public function write($writer)
	{
		$writer->writeValue(0, $this->year);
		$writer->writeValue(1, $this->month);
		$writer->writeValue(2, $this->day);
		$writer->writeValue(3, $this->hour);
		$writer->writeValue(4, $this->minute);
		$writer->writeValue(5, $this->second);
	}
}
?>