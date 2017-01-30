using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Principal;
using Lithnet.ResourceManagement.Client;
using Microsoft.ResourceManagement.WebServices;
using System.Diagnostics;

namespace Lithnet.ResourceManagement.UI.UserVerification
{
    public static class UserUtils
    {
        private static ResourceManagementClient client;

        private static ResourceManagementClient Client
        {
            get
            {
                if (UserUtils.client == null)
                {
                    UserUtils.client = new ResourceManagementClient();
                }

                return UserUtils.client;
            }
        }

        internal static bool IsMemberOfSet(ResourceObject o, Guid setId)
        {
            return UserUtils.IsMemberOfSet(o.ObjectID, setId);
        }

        internal static bool IsMemberOfSet(UniqueIdentifier userId, Guid setId)
        {
            string xpathFilter = $"/Person[(ObjectID = '{userId.Value}') and (ObjectID = /Set[ObjectID = '{setId}']/ComputedMember)]";

            return UserUtils.CountResources(xpathFilter) > 0;
        }

        internal static bool IsMemberOfSet(UniqueIdentifier userId, string setName)
        {
            string xpathFilter = $"/Person[(ObjectID = '{userId.Value}') and (ObjectID = /Set[DisplayName = '{setName}']/ComputedMember)]";

            return UserUtils.CountResources(xpathFilter) > 0;
        }

        private static int CountResources(string xpathFilter)
        {
            int count = UserUtils.Client.GetResourceCount(xpathFilter);

            if (count == 0)
            {
                Trace.WriteLine($"No members found for query: {xpathFilter}");
            }
            else
            {
                Trace.WriteLine($"{count} members found for query: {xpathFilter}");
            }

            return count;
        }
    

        internal static bool IsMemberOfSet(ResourceObject o, string setName)
        {
            return UserUtils.IsMemberOfSet(o.ObjectID, setName);
        }

        internal static ResourceObject GetCurrentUser()
        {
            return UserUtils.GetCurrentUser(new string[] {"ObjectID"});
        }

        internal static ResourceObject GetCurrentUser(IEnumerable<string> attributesToGet)
        {
            string username = WindowsIdentity.GetCurrent().Name;

            Trace.WriteLine($"Getting current user {username} from FIM service");

            string[] parts = username.Split('\\');

            Dictionary<string, object> valuePairs = new Dictionary<string, object>();
            valuePairs.Add("Domain", parts[0]);
            valuePairs.Add("AccountName", parts[1]);

            ResourceObject o = UserUtils.Client.GetResourceByKey("Person", valuePairs, attributesToGet);

            if (o == null)
            {
                Trace.WriteLine($"User {username} was not found");
            }
            else
            {
                Trace.WriteLine($"User {username} was found with ObjectID {o.ObjectID.Value}");
            }

            return o;
        }
    }
}