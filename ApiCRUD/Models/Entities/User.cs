using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System;
using ApiCRUD.Models.Enums;

namespace ApiCRUD.Models.Entities
{
    public class User
    {
        [Key]
        public Guid Guid { get; set; }

        private string _login;
        public string Login
        {
            get => _login;
            set
            {
                if (Regex.IsMatch(value, "^[A-Za-z0-9]*$")) _login = value;
                /*else "error";*/
            }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set
            {
                if (Regex.IsMatch(value, "^[A-Za-z0-9]*$")) _password = value;
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (Regex.IsMatch(value, "^[A-Za-zА-Яа-я]*$")) _name = value;
            }
        }

        public Gender Gender { get; set; }

        public DateTime? BirthDay { get; set; }

        public bool Admin { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedBy { get; set; }
        
        public DateTime? ModifiedOn { get; set; }
        
        public string ModifiedBy { get; set; } = null;
        
        public DateTime? RevokedOn { get; set; }
        
        public string RevokedBy { get; set; } = null;
    }
}
