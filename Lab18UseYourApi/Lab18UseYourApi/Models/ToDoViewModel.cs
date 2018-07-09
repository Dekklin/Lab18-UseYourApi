using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lab18UseYourApi.Models
{
    public class ToDoViewModel
    {
        public List<ToDo> ToDos;
        public ToDoList List { get; set; }
    }
}
