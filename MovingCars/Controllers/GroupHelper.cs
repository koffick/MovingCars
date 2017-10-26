using MovingCars.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace MovingCars.Controllers
{
    public class GroupHelper
    {
        //var groups = GroupHelper.Groups();
        //var users = GroupHelper.GetGroupFIOUsers("TMN\\Пользователи домена");


        /// <summary>
        /// Получить все локальные группы
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> Groups()
        {
            foreach (IdentityReference group in HttpContext.Current.Request.LogonUserIdentity.Groups)
            {
                string result = group.Translate(typeof(NTAccount)).ToString();
                yield return result;
            }
        }

        /// <summary>
        /// Получить все группы домена
        /// </summary>
        /// <param name="DomainName"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetDomainGroups(string DomainName)
        {
            using (var context = new PrincipalContext(ContextType.Domain, DomainName))
            using (var queryFilter = new GroupPrincipal(context))
            using (var searcher = new PrincipalSearcher(queryFilter))
            {
                foreach (var result in searcher.FindAll())
                {
                    yield return result.SamAccountName;
                    result.Dispose();
                }
            }
        }

        /// <summary>
        /// Принадлежит ли пользователь группе
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static bool IsUserMemberOf(string userName, string groupName)
        {
            using (var ctx = new PrincipalContext(ContextType.Domain))
            using (var groupPrincipal = GroupPrincipal.FindByIdentity(ctx, groupName))
            using (var userPrincipal = UserPrincipal.FindByIdentity(ctx, userName))
            {
                return userPrincipal.IsMemberOf(groupPrincipal);
            }
        }

        /// <summary>
        /// Группы, в которые входит пользователь
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetUserGroups(string userName)
        {
            using (var ctx = new PrincipalContext(ContextType.Domain))
            using (var userPrincipal = UserPrincipal.FindByIdentity(ctx, userName))
            {
                return userPrincipal.GetGroups().Select(d => d.SamAccountName).ToList();
            }
        }

        /// <summary>
        /// Пользователи, которые входят в группу
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetGroupUsers(string groupName)
        {
            using (var ctx = new PrincipalContext(ContextType.Domain))
            using (var groupPrincipal = GroupPrincipal.FindByIdentity(ctx, groupName))
            {
                return groupPrincipal.Members.Select(d => d.SamAccountName).ToList();
            }
        }

        /// <summary>
        /// Пользователи, которые входят в группу
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public static IEnumerable<Passenger> GetGroupFIOUsers(string groupName)
        {
            List<Passenger> passengers = new List<Passenger>();
            using (var ctx = new PrincipalContext(ContextType.Domain))
            using (var groupPrincipal = GroupPrincipal.FindByIdentity(ctx, groupName))
            {
                foreach (var item in groupPrincipal.Members.ToList())
                {
                    using (var userPrincipal = UserPrincipal.FindByIdentity(ctx, IdentityType.SamAccountName, item.SamAccountName))
                    {
                        if (userPrincipal.DisplayName != null && !IsStringLatin(userPrincipal.DisplayName))
                        {
                            var person = new Passenger
                            {
                                FirstName = GetFirstWord(userPrincipal.GivenName),
                                LastName = GetFirstWord(userPrincipal.Surname),
                                Patronymic = GetPatronymic(userPrincipal.DisplayName, GetFirstWord(userPrincipal.GivenName), GetFirstWord(userPrincipal.Surname), userPrincipal.MiddleName),
                                Phone = userPrincipal.VoiceTelephoneNumber,
                                Department = GetDepartment(userPrincipal.DistinguishedName)
                            };
                            passengers.Add(person);
                        }
                    }
                }

                return passengers;
            }
        }

        private static string GetFirstWord(string source)
        {
            string destination = source;
            if (source != null && source.Split(' ').Length > 1)
            {
                destination = source.Split(' ')[0];
            }
            return destination;
        }

        private static string GetPatronymic(string fio, string firstName, string lastName, string patronymic)
        {
            if (patronymic == null)
            {
                List<string> strings = fio.Split(' ').ToList();
                strings.Remove(firstName);
                strings.Remove(lastName);
                if (strings.Count == 1)
                {
                    patronymic = strings.First();
                }
            }
            return patronymic;
        }

        private static string GetDepartment(string source)
        {
            string destination = "";
            if (source != null)
            {
                foreach (var item in source.Split(','))
                {
                    if (!IsStringLatin(item) && (item.Contains("OU=")))
                    {
                        destination += ", " + item.Replace("OU=","");
                    }
                }
            }
            if (destination.Length > 1 && destination.Substring(0, 2) == ", ")
            {
                destination = destination.Substring(2, destination.Length - 2);
            }
            return destination;
        }

        private static bool IsStringLatin(string content)
        {
            bool result = true;

            char[] letters = content.ToCharArray();

            for (int i = 0; i < letters.Length; i++)
            {
                int charValue = System.Convert.ToInt32(letters[i]);

                if (charValue > 128)
                {
                    result = false;
                    break;
                }
            }
            return result;
        }
    }
}