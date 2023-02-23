using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wsl_partition
{
    internal class GptType
    {
        public static Dictionary<string, string> Gpt = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {

            /* Linux (https://systemd.io/DISCOVERABLE_PARTITIONS/) */
            //{"0657FD6D-A4AB-43C4-84E5-0933C84B4F4F", "Linux swap"},
            {"0FC63DAF-8483-4772-8E79-3D69D8477DE4", "ext4"},
            {"E6D6D379-F507-44C2-A23C-238F2A3DF928", "lvm"},
            /* ... too crazy, ignore for now:
                {"7FFEC5C9-2D00-49B7-8941-3EA10A5586B7", ("Linux plain dm-crypt")},
                {"CA7D7CCB-63ED-4C53-861C-1742536059CC", ("Linux LUKS")},*/

            /* FreeBSD */
            /*{"516E7CB4-6ECF-11D6-8FF8-00022D09712B", "FreeBSD data"},
            {"83BD6B9D-7F41-11DC-BE0B-001560B84F0F", "FreeBSD boot"},
            {"516E7CB5-6ECF-11D6-8FF8-00022D09712B", "FreeBSD swap"},
            {"516E7CB6-6ECF-11D6-8FF8-00022D09712B", "FreeBSD UFS"},
            {"516E7CBA-6ECF-11D6-8FF8-00022D09712B", "FreeBSD ZFS"},
            {"516E7CB8-6ECF-11D6-8FF8-00022D09712B", "FreeBSD Vinum"},*/
        };

    }
}
