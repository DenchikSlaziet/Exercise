using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Work.Models
{
    /// <summary>
    /// Сущность человека
    /// </summary>
    public class People
    {
        public Guid Id { get; set; }

        /// <summary>
        /// ФИО
        /// </summary>
        public string FIO { get; set; }

        /// <summary>
        /// Дата рождения
        /// </summary>
        public string DateOfBirth { get; set; }

        /// <summary>
        /// Пол
        /// </summary>
        public string Gender { get; set; }

        /// <summary>
        /// Возраст
        /// </summary>
        public int Age => DateTime.Now.Date.Year - Convert.ToDateTime(DateOfBirth).Date.Year;

        public People(string fio, DateTime data, Gender gender)
        {
            Id = Guid.NewGuid();
            FIO = fio;
            Gender = gender.ToString();
            DateOfBirth = data.ToShortDateString();
        }
        public People()
        {

        }

        public override string ToString()
        {
            return FIO;
        }
    }
}
