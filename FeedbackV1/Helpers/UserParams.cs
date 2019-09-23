using System;

namespace FeedbackV1
{
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber {get; set;} = 1;
        private int pageSize = 10;
         public int PageSize  
        {
            get { return pageSize; }
            set { pageSize = (value>MaxPageSize) ? MaxPageSize : value ;}
        }
        
        public string UserId { get; set; }
        public string Role { get; set; } = null;
        private Boolean myTeam = false;
        public Boolean Team
        {
            get { return myTeam; }
            set { myTeam = value; }
        }
        public string OrderBy { get; set; }
        public string Manager { get; set; }
        public string Search { get; set; } = "";
        public Boolean Pending { get; set; } = false; // pentru ReceiverController

        
    }
}