using System;
using Sitana.Framework.Cs;

namespace GameEditor
{
    public class Layer
    {
        public SharedString Name { get; private set;}
        public string Type { get; private set;}

        public SharedValue<bool> Selected { get; private set;}

        public SharedValue<bool> NotSelected { get; private set;}

        public Layer(string type)
        {
            Name = new SharedString();
            Type = type;
            Selected = new SharedValue<bool>(false);
            NotSelected = new SharedValue<bool>(false);

            Selected.ValueChanged += (bool newValue) => 
            {
                NotSelected.Value = !newValue;
            };
        }
    }
}

