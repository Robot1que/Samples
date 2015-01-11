using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DataGridStyle
{
    [DebuggerDisplay("Title = {Title}")]
    public class CountryItem
    {
        private readonly string _title;
        public string Title
        {
            get
            {
                return this._title;
            }
        }

        public CountryItem(string title)
        {
            this._title = title;
        }
    }
}
