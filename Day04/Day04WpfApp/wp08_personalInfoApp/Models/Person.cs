using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wp08_personalInfoApp.Logics;

namespace wp08_personalInfoApp.Models
{
    internal class Person
    {
        // 외부에서 접근불가
        
        private string email;
        private DateTime date;

        public string FirstName { get ; set; }
        public string LastName { get ; set; }
        public string Email 
        { 
            get => email;
            set 
            {
                if (Commons.IsValidEmail(value) != true) // 이메일은 형식에 일치안함 
                {
                    throw new Exception("유효하지 않은 이메일 형식");
                }
                else
                {
                    email = value;
                }
            }
        }
        public DateTime Date 
        { 
            get => date;
            set
            {
                var result = Commons.GetAge(value);
                if (result > 120 || result <= 0)
                {
                    throw new Exception("유효하지 않은 생일");
                }
                else
                {
                    date = value;
                }
            }
        }

        public bool IsAdult
        {
            get
            {
                return Commons.GetAge(date) > 18;       // 19살 이상이면 true
            }
        }

        public bool IsBirthday
        {
            get
            {
                return DateTime.Now.Month == date.Month && DateTime.Now.Day == date.Day;
            }
        }

        public string Zodiac
        {
            get => Commons.GetZodiac(date);
        }
        
        public Person(string firstName, string lastName, string email, DateTime date)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Date = date;
        }
    }
}
