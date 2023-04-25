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
        // 외부에서 접근 불가
        // private string firstName;
        // private string lastName;
        private string email;
        private DateTime date;

        // 외부에서 접근하기 위한 property
        public string FirstName { get; set; } // Auto Property 되면 위의 private string firstName 필요 없음
        public string LastName { get; set; }
        public string Email
        { 
            get => email;
            set 
            {
                if (Commons.IsValidEmail(value) != true) // 이메일 형식에 일치하지 않음
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
                    throw new Exception("유효하지 않은 생년월일");
                }
                else
                {
                    date = value;
                }
            }
        }

        public bool IsAult
        {
            //get
            //{
            //    return Commons.GetAge(date) > 18; // 나이가 만 18세가 넘는 경우 true 
            //}
            get => Commons.GetAge(date) > 18;      // 람다식 방법
        }

        public bool IsBirthDay
        {
            get
            {
                return DateTime.Now.Month == date.Month &&
                       DateTime.Now.Day == date.Day;
            }
        }

        public string Zodiac
        {
            get
            {
                return Commons.GetZodiac(date);
            }

            //get => Commons.GetZodiac(date);  // 람다식 방법
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
