using System;
using Sitana.Framework.Cs;
using Sitana.Framework.Games;

namespace GameEditor
{
    public class DocLayer
    {
        public SharedString Name { get; private set;}
        public string Type { get; private set;}

        public SharedValue<bool> Selected { get; private set;}

        public SharedValue<bool> NotSelected { get; private set;}

        protected Layer _layer = null;

        public DocLayer(string type)
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

