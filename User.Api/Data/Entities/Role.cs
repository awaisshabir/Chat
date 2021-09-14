using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace User.Api.Data.Entities
{
    public class Role
    {
        public int RoleId { get; set; }
        public string RoleName { get; private set; }
        public Role()
        {

        }
        public Role(string roleName)
        {
            this.RoleName = roleName;
        }
    }
}
