using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hope_National_Hospital.Application.Constants
{
    public class CacheKeys
    {
        public const string Departments = "departments";
        public static string Department(int id)  => $"department:{id}";

        public const string Rooms = "rooms";
        public const string AvailableBeds = "available_beds";
        public const string Doctors = "doctors";

        public static string Doctor(int id)
            => $"doctor:{id}";
    }
}
