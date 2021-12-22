using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagement.WEB.Authorization
{
    public class ProjectAdminOrCreatorRequirement : IAuthorizationRequirement
    {
        public ProjectAdminOrCreatorRequirement()
        {

        }
    }
}
