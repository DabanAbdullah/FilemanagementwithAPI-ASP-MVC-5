using Microsoft.Owin.Security.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using project.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace project
{
   

        public class TokenGenerating : OAuthAuthorizationServerProvider
        {
            public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
            {
                context.Validated(); //
            }

            public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
            {
                UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                var result = userManager.Find(context.UserName, context.Password);
                
                if (result != null)
                {
                    var identity = new ClaimsIdentity(context.Options.AuthenticationType);
               
              
                identity.AddClaim(new Claim(ClaimTypes.Name, result.UserName));
                identity.AddClaim(new Claim(ClaimTypes.Email, result.Id));
                context.Validated(identity);
            }
            else
                {
                    context.SetError("invalid_grant", "Provided username and password is incorrect");
                    return;
                }


            //UserManager<IdentityUser> userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>());
            //var result = userManager.Find(context.UserName, context.Password);
            //UserManager holds data for register user.
            //context.UserName = Email of your registered user
            //context.Password = Password of your registered user
        }
    }
    }
