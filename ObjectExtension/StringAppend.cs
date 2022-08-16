using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectExtension
{
    public class StringAppend
    {
        public StringAppend()
        {
            this.ValueList = new List<string>();
        }
        public StringAppend(string value)
        {
            this.ValueList = new List<string>();
            this.ValueList.Add(value);
        }
        public StringAppend(int value)
        {
            this.ValueList = new List<string>();
            this.ValueList.Add(value.ToString());
        }
        public List<string> ValueList { get; set; }

        public override string ToString()
        {
            return string.Join(string.Empty, ValueList);
        }

        /// <summary>
        /// get final string which concat all appended string by <paramref name="concatBy"/>
        /// </summary>
        /// <param name="concatBy"></param>
        /// <returns></returns>
        public string ToString(string concatBy)
        {
            return string.Join(concatBy, ValueList);
        }

        /// <summary>
        /// get final string which concat all appended string
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public StringAppend Append(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                this.ValueList.Add(value);
            }
            return this;
        }

        public StringAppend Append(int value)
        {
            this.ValueList.Add(value.ToString());
            return this;
        }
    }
}
