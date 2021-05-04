using System;
using System.Linq;

namespace AutoConfig
{
    public class AutoConfigAttribute : Attribute
    {
        public string[] Sections { get; set; }

        public AutoConfigAttribute(params string[] sections)
        {
            Sections = sections;
        }

        public string GetSectionKey()
        {
            if (Sections == null || Sections.Length == 0)
            {
                return null;
            }
            return Sections.Length == 1 ? Sections[0] : Sections.Aggregate((x, y) => x + ":" + y);
        }
    }
}
