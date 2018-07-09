using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lab18UseYourApi.Models
{
    public class ToDoListViewModel
    {
        public List<ToDoList> TodoLists { get; set; }

        [Display(Name = "To-Do's")]
        public IEnumerable<ToDo> TodoItems { get; set; }
    }
}
