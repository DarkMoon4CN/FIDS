using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CATC.FIDS.Model
{
    public class TemplateBase
    {

        private Guid _id;

        public int Tag { get; set; }
        public string Style { get; set; }
        public string Class { get; set; }
        public Guid ID
        {
            set { _id = value; }
            get
            {
                if (_id == null)
                {
                    return System.Guid.NewGuid();
                }
                return _id;
            }
        }
    }

}
