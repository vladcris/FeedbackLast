using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Net.Http;
using System.Net;
using FeedbackV1.Models;
using System.Security.Claims;
using FeedbackV1.Dtos;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace FeedbackV1.Repositories
{
    public class TableStorageRepository : IAuthRepository
    {
        private CloudTable cardsTable, feedbacksTable, userTable, departamentTable;
        private CloudTableClient tableClient;
        const string StorageAccountName = "employeesfeedbackapp";
        const string StorageAccountKey = "tTQ64T+PDy/lIqusmfzmvkMh/nMQKPDVd7w8pzJ8fpKgMTQal75PtIdjQU+0gg3cpDNasv5DwQygDJjN3wS5eg==";
        public TableStorageRepository()
        {
            CloudStorageAccount storageAccount = new CloudStorageAccount(new Microsoft.WindowsAzure.Storage.Auth.StorageCredentials(StorageAccountName, StorageAccountKey), true);
            tableClient = storageAccount.CreateCloudTableClient();
            cardsTable = tableClient.GetTableReference("Employees");
            feedbacksTable = tableClient.GetTableReference("Feedbacks");
            userTable = tableClient.GetTableReference("User");
            departamentTable = tableClient.GetTableReference("Departments");
            // Create the CloudTable if it does not exist
        }
        public async Task PostEntityFeedback(ITableEntity obj)
        {
            TableOperation insertOperation = TableOperation.Replace(obj);
            try
            {
                var result = await feedbacksTable.ExecuteAsync(insertOperation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task PostEntityUser(ITableEntity obj)
        {
            TableOperation insertOperation = TableOperation.InsertOrReplace(obj);
            try
            {
                var result = await userTable.ExecuteAsync(insertOperation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task<List<Employees>> GetAllEntities()
        {
            await cardsTable.CreateIfNotExistsAsync();
            var results = (await cardsTable.ExecuteQuerySegmentedAsync(new TableQuery<Employees>(), null)).ToList<Employees>();
            List<Employees> people = results;
            people = people.OrderBy(x => x.Name).ToList();
            if (people == null)
                return null;
            return people;
        }

         public async Task<List<Departament>> GetAllDepartments()
        {
            await cardsTable.CreateIfNotExistsAsync();
            var results = (await departamentTable.ExecuteQuerySegmentedAsync(new TableQuery<Departament>(), null)).ToList<Departament>();
            List<Departament> people = results;
            people = people.OrderBy(x => x.Name).ToList();
            if (people == null)
                return null;
            return people;
        }

        
        public async Task<List<Employees>> GetAllEntities2(string value)
        {   
           
            await cardsTable.CreateIfNotExistsAsync();
            // string filter1 = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, value);
            string filter2 = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, value);
            // string combinedFilter = TableQuery.CombineFilters(filter1, TableOperators.And, filter2);
            TableQuery<Employees> query = new TableQuery<Employees>().Where(filter2);
            var result = await cardsTable.ExecuteQuerySegmentedAsync(query,null);
             if (result == null)
                return null;
            var results = result.Results;
            return results;
        }
        public async Task<List<Employees>> GetEntityById(string id)
        {
            await cardsTable.CreateIfNotExistsAsync();
            var filter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id);
            TableQuery<Employees> query = new TableQuery<Employees>().Where(filter);
            var result = await cardsTable.ExecuteQuerySegmentedAsync(query, null);
            if (result == null)
                return null;
            var results = result.Results;
            return results;
        }

         public async Task<List<Feedbacks>> GetAllFeedbacks()
        {
            await feedbacksTable.CreateIfNotExistsAsync();
            var results = (await feedbacksTable.ExecuteQuerySegmentedAsync(new TableQuery<Feedbacks>(), null)).ToList();
            List<Feedbacks> feedbacks = results;
            feedbacks = feedbacks.OrderByDescending(x => x.Pending).ToList();
             if (feedbacks == null)
                return null;
            return feedbacks;
        }

        public async Task<Feedbacks> GetFeedByFeedId(string id)
        {
            await feedbacksTable.CreateIfNotExistsAsync();
            var filter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id);
            TableQuery<Feedbacks> query = new TableQuery<Feedbacks>().Where(filter);
            var result = await feedbacksTable.ExecuteQuerySegmentedAsync(query, null);
            if (result == null)
                return null;
            // var results = result.Results;
            return result.FirstOrDefault();
        }

        
        public async Task<PagedList<Feedbacks>> GetMyFeedbacks(UserParams userParams, string id)
        {
            await feedbacksTable.CreateIfNotExistsAsync();
            var filter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id);
            TableQuery<Feedbacks> query = new TableQuery<Feedbacks>().Where(filter);
            var result = await feedbacksTable.ExecuteQuerySegmentedAsync(query, null);

            List<Feedbacks> feedbacks = result.Results;
            feedbacks = feedbacks.OrderByDescending(x => x.Pending).ThenByDescending(x => x.Timestamp).ToList();

            var list = new List<Feedbacks>();
            list = feedbacks;
            var queryable = list.AsQueryable();

            var paginatedResult = PagedList<Feedbacks>.Create(queryable, userParams.PageNumber, userParams.PageSize);

            if (paginatedResult == null)
                return null;

            return paginatedResult;

        }

        public async Task<Feedbacks> GetNamesForFeedback(string id)
        {   
            var repo = new TableStorageRepository();

            var feedback = await repo.GetFeedByFeedId(id);
            var users = await repo.GetUsersWithoutParams();

            var names = new Feedbacks {};

            foreach (var user in users)
            {
                if (user.Id == feedback.ID)
                {
                    feedback.ID = user.Name;
                }

                if (user.Id == feedback.ID_receiver)
                {
                    feedback.ID_receiver = user.Name;
                }

                if (user.Id == feedback.ID_manager)
                {
                    feedback.ID_manager = user.Name;
                }

            }

            return feedback;
        }


        public async Task<Feedbacks> GetFeedByFeedAndUserId(string id, string feed)
        {
            await feedbacksTable.CreateIfNotExistsAsync();
            var filter1 = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id);
            var filter2 = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, feed);
            var combinedFilter = TableQuery.CombineFilters(filter1, TableOperators.And, filter2);
            TableQuery<Feedbacks> query = new TableQuery<Feedbacks>().Where(combinedFilter);
            var result = await feedbacksTable.ExecuteQuerySegmentedAsync(query, null);
            if (result == null)
                return null;
            return result.FirstOrDefault();
        }
        
        public async Task<Feedbacks> GiveFeedback(Feedbacks feedback)
        {   
            feedback.RowKey = Guid.NewGuid().ToString();
            await feedbacksTable.CreateIfNotExistsAsync();
            TableOperation insertOperation = TableOperation.InsertOrReplace(feedback);
            var result = await feedbacksTable.ExecuteAsync(insertOperation);
            return(feedback);

        }

        public async Task<Feedbacks> RequestFeedback(Feedbacks feedback, string id)
        {
            feedback.RowKey = Guid.NewGuid().ToString();
            feedback.PartitionKey = id;
            await feedbacksTable.CreateIfNotExistsAsync();
            TableOperation insertOperation = TableOperation.InsertOrReplace(feedback);
            var result = await feedbacksTable.ExecuteAsync(insertOperation);
            return (feedback);

        }

        public async Task<PagedList<Feedbacks>> GetFeedbacksByReceiver(UserParams userParams,string id)
        {
            await feedbacksTable.CreateIfNotExistsAsync();
            var filter = TableQuery.GenerateFilterCondition("ID_receiver", QueryComparisons.Equal, id);
            TableQuery<Feedbacks> query = new TableQuery<Feedbacks>().Where(filter);

            var result = await feedbacksTable.ExecuteQuerySegmentedAsync(query, null);
            if (result == null)
                return null;
            var results = result.Results.AsQueryable();
            results = results.OrderByDescending(x => x.Pending).ThenByDescending(x => x.Timestamp);

            if(userParams.Pending) {
                results = results.Where(x => x.Pending == false);
            }
            
            var paginatedResult = PagedList<Feedbacks>.Create(results, userParams.PageNumber, userParams.PageSize);

            if (paginatedResult == null)
                return null;

            return paginatedResult;

        }

        

        // public async Task<IEnumerable<PagedList<Feedbacks>>> GetFeedbacksWithName(PagedList<Feedbacks> feedbacks)
        // {
        //     var repo = new TableStorageRepository();
        //     var users = await repo.GetUsersWithoutParams();
            

        //     foreach (var feedback in feedbacks)
        //     {
        //         foreach (var user in users)
        //         {
                    
        //         if (user.Id == feedback.ID)
        //         {
        //             feedback.Sender = user.Name;
        //         }

        //         if (user.Id == feedback.ID_receiver)
        //         {
        //             feedback.Receiver = user.Name;
        //         }

        //         if (user.Id == feedback.ID_manager)
        //         {
        //             feedback.Manager = user.Name;
        //         }

        //         }
        //     }
        //     return feedbacks;
        // }


        //////User////
        public async Task<User> Register(User user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHAsh(password, out passwordHash, out passwordSalt);

            user.PartitionKey = user.Dep_Id;
            user.RowKey = Guid.NewGuid().ToString();
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await userTable.CreateIfNotExistsAsync();
            //TableOperation insertOperation = TableOperation.Insert(user);
           // await userTable.ExecuteAsync(insertOperation);
            TableOperation insertOperation = TableOperation.InsertOrReplace(user);
             var result = await userTable.ExecuteAsync(insertOperation);
            return (user);

        }

        private void CreatePasswordHAsh(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
            
        }

        public async Task<User> Login(string email, string password)
        {
        
            await userTable.CreateIfNotExistsAsync();
            var filter = TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email);
            TableQuery<User> query = new TableQuery<User>().Where(filter);
            var result = await userTable.ExecuteQuerySegmentedAsync(query, null);
            var user =  result.FirstOrDefault();

             if (user == null)
                return null;
            
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
            return null;

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != passwordHash[i]) return false;
                }
            }
            return true;
        }

        public async Task<bool> UserExists(string email)
        {   
            await userTable.CreateIfNotExistsAsync();
            var filter = TableQuery.GenerateFilterCondition("Email", QueryComparisons.Equal, email);
            TableQuery<User> query = new TableQuery<User>().Where(filter);
            var result = await userTable.ExecuteQuerySegmentedAsync(query, null);
            var user =  result.FirstOrDefault();

            if(user == null)
            return false;

            return true;
        }


        /////user apisss
        public void Add<T>(T entity) where T : class
        {
            var repo = new TableStorageRepository();
            repo.Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            var repo = new TableStorageRepository();
            repo.Delete(entity);
        }

        public async Task<bool> SaveAll()
        {
            var repo = new TableStorageRepository();
            return await repo.SaveAll();
        }


        public async Task<IEnumerable<User>> GetUsersWithoutParams()
        {
            await userTable.CreateIfNotExistsAsync();

            var users = (await userTable.ExecuteQuerySegmentedAsync(new TableQuery<User>(), null)).AsQueryable();
            users = users.Where(x => x.IsDeleted == false).OrderByDescending(x => x.Name);
             if (users == null)
                return null;
            return users;
        }

        public async Task<IEnumerable<User>> GetUsersWithoutParamsForAdmin()
        {
            await userTable.CreateIfNotExistsAsync();
            
            var users = (await userTable.ExecuteQuerySegmentedAsync(new TableQuery<User>(), null)).AsQueryable();
            users = users.OrderByDescending(x => x.IsDeleted);
             if (users == null)
                return null;
            return users;
        }
           

        public async Task<PagedList<User>> GetUsers(UserParams userParams)
        {   
            var repo = new TableStorageRepository();
            await userTable.CreateIfNotExistsAsync();

            var results = (await userTable.ExecuteQuerySegmentedAsync(new TableQuery<User>(), null)).AsQueryable();
            
            results = results.Where(x => x.IsDeleted == false).Where(x => x.Role != "admin").Where(x => x.RowKey != userParams.UserId).OrderBy(x => x.Name);

            if ((!string.IsNullOrEmpty(userParams.Role) ) && userParams.Team)
            {
                if(userParams.Role == "manager"){

                    var newResults = await repo.GetMyTeamAsManager(userParams.UserId);
                    results = newResults.AsQueryable().OrderByDescending(x => x.Name);
                                                      
                }else if (userParams.Role == "employee") 
                {
                    var newResults = await repo.GetMyTeamAsEmployee(userParams.Manager);
                    results = newResults.AsQueryable().Where(x => x.RowKey != userParams.UserId).Union(results.AsQueryable()
                                                      .Where(x => x.Name == userParams.Manager)).OrderByDescending(x => x.Name);
                }else {
                    userParams.Role = null;
                }
            }

            if (!string.IsNullOrEmpty(userParams.Search)) 
            {   
                
                results = results.Where(x => x.Name.ToLower().Contains(userParams.Search.ToLower()) ||
                                             x.Manager_ID.ToLower().Contains(userParams.Search.ToLower()) ||
                                             x.Email.ToLower().Contains(userParams.Search.ToLower())
                                            );
            }
          

            if (!string.IsNullOrEmpty(userParams.OrderBy))
            {
                switch(userParams.OrderBy)
                {
                    case "desc":
                        results = results.OrderByDescending(u => u.Name);
                        break;
                    case "asc":
                        results = results.OrderBy(u => u.Name);
                        break;
                    case "man-asc":
                        results = results.OrderBy(u => u.Manager_ID);
                        break;
                    case "man-desc":
                        results = results.OrderByDescending(u => u.Manager_ID);
                        break;
                    default:
                        results = results.OrderBy(u => u.Name);
                        break;

                }
            }

            
            if (results == null)
                return null;

            var paginatedResult = PagedList<User>.Create(results, userParams.PageNumber, userParams.PageSize);

            if (paginatedResult == null)
                return null;
            return paginatedResult;
        }

        public async Task<User> GetUser(string id)
        {   

            await userTable.CreateIfNotExistsAsync();
            var filter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, id);
            TableQuery<User> query = new TableQuery<User>().Where(filter);
            var result = await userTable.ExecuteQuerySegmentedAsync(query, null);
            if (result == null)
                return null;
            return result.FirstOrDefault();
        }

        public async Task<IEnumerable<User>> GetMyTeamAsEmployee(string name)
        {   

            await userTable.CreateIfNotExistsAsync();
            var filter1 = TableQuery.GenerateFilterCondition("Manager_ID", QueryComparisons.Equal, name);
            //var filter2 = TableQuery.GenerateFilterCondition("IsDeleted", QueryComparisons.Equal, "false");
           // string combinedFilter = TableQuery.CombineFilters(filter1, TableOperators.And, filter2);

            TableQuery<User> query = new TableQuery<User>().Where(filter1);
            var result = await userTable.ExecuteQuerySegmentedAsync(query, null);

            if (result == null)
                return null;
            var results = result.Results;
           
            List<User> users = result.Results;
            users = users.Where(x => x.IsDeleted == false).ToList();

            return users;
        }

        
        public async Task<List<User>> GetUsersByDepartment(string id)
        {
            await userTable.CreateIfNotExistsAsync();
            var filter =  TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, id);
            TableQuery<User> query = new TableQuery<User>().Where(filter);
            var result = await userTable.ExecuteQuerySegmentedAsync(query, null);
            if (result == null)
                return null;
            var results = result.Results;
            List<User> users = result.Results;
            users = users.Where(x => x.IsDeleted == false).ToList();
            return users;
        }

        public async Task<IEnumerable<User>> GetAllManagers()
        {
            await userTable.CreateIfNotExistsAsync();

            var users = (await userTable.ExecuteQuerySegmentedAsync(new TableQuery<User>(), null)).AsQueryable();

            var results = users.Where(x => x.Role == "manager").OrderBy(item => item.Name).Where(x => x.IsDeleted == false); 

            return results;


        }


         public async Task DeleteUser(ITableEntity obj)
        {
            TableOperation insertOperation = TableOperation.Delete(obj);
            try
            {
                var result = await userTable.ExecuteAsync(insertOperation);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

         public async Task<IEnumerable<User>> GetUsersForTeam()  
        {
            await userTable.CreateIfNotExistsAsync();
            var results = (await userTable.ExecuteQuerySegmentedAsync(new TableQuery<User>(), null)).ToList<User>();

            List<User> people = results;
            people = people.OrderByDescending(x => x.Name).Where(x => x.IsDeleted == false).ToList();

            if (people == null)
                return null;  
            return people;
        }

        public async Task<IEnumerable<User>> GetMyTeamAsManager(string managerid)
        {   
            var repo = new TableStorageRepository();
            var users = await repo.GetUsersForTeam();

            Dictionary<string, List<Models.User>> descendantsFromManager = new Dictionary<string, List<Models.User>>();
            Models.User manager = new Models.User();
            manager.Id = managerid;
            manager.Name = managerid;
            foreach (var user in users)
            {
                if (!descendantsFromManager.ContainsKey(user.Manager_ID))
                    descendantsFromManager[user.Manager_ID] = new List<Models.User>();
                descendantsFromManager[user.Manager_ID].Add(user);

                if (user.Name == managerid || user.Id == managerid)
                    manager = user;
               
                
            }
            Queue<Models.User> queue = new Queue<Models.User>();
            List<Models.User> descendants = new List<Models.User>();
            
            queue.Enqueue(manager);
            while(queue.Count!=0)
            {
                var user = queue.Dequeue();
                if (user.Name != managerid && user.Id != managerid)
                    descendants.Add(user);
                if(descendantsFromManager.ContainsKey(user.Name))
                {
                    foreach(var child in descendantsFromManager[user.Name])
                    {
                        queue.Enqueue(child);
                       
                    }
                    descendantsFromManager.Remove(user.Name);
                }
            }   

            
        
            // descendants = descendants.Where(x => x.IsDeleted == false).ToList();

            return descendants;
        }



        static async Task Execute(User sender, User receiver)
        {
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(sender.Email, sender.Name);
            var subject = "Sending with Twilio SendGrid is Fun";
            var to = new EmailAddress(receiver.Email, receiver.Name);
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg); 
        }
    }
}