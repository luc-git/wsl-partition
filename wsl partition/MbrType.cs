using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wsl_partition
{
    internal class MbrType
    {
        public static Dictionary<string, string> Mbr = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
	        //{"0x82", "Linux swap / Solaris"},
            {"0x83", "ext4"},
	        /*{"0x85", "Linux extended"},
            {"0x88", "Linux plaintext"},*/
            {"0x8e", "lvm"},
            /*{"0xa5", "FreeBSD"},
	        {"0xa6", "OpenBSD"},*/
        };
    }
}
