using Lab18UseYourApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lab18UseYourApi.Controllers
{
    public class ToDoController : Controller
    {
        public async Task<IActionResult> Index(string searchString)
        {
            using (var client = new HttpClient())
            {
                // add the appropriate properties on top of the client base address.
                client.BaseAddress = new Uri("http://lab17-webapi5565.azurewebsites.net/");

                //the .Result is important for us to extract the result of the response from the call
                var response = client.GetAsync("api/Todos/").Result;
                if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    var stringResult = await response.Content.ReadAsStringAsync();
                    List<ToDo> Todo = JsonConvert.DeserializeObject<List<ToDo>>(stringResult);
                    var toDos = from t in Todo
                                select t;

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        toDos = toDos.Where(s => s.Title.Contains(searchString));
                    }

                    var todoVM = new ToDoViewModel();
                    todoVM.ToDos = toDos.ToList();

                    return View(todoVM);
                }
                return View();
            }
        }

        /// <summary>
        /// GET: TodoItem/Details/id#
        /// </summary>
        /// <param name="id">the ID# of the item to get details</param>
        /// <returns>view</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            using (var client = new HttpClient())
            {

                try
                {
                    // add the appropriate properties on top of the client base address.
                    client.BaseAddress = new Uri("http://lab17-webapi5565.azurewebsites.net/");

                    //the .Result is important for us to extract the result of the response from the call
                    var itemResponse = client.GetAsync($"/api/todos/{id}").Result;
                    var listsResponse = client.GetAsync($"/api/todolist/").Result;

                    if (itemResponse.EnsureSuccessStatusCode().IsSuccessStatusCode
                        && listsResponse.EnsureSuccessStatusCode().IsSuccessStatusCode)
                    {
                        var itemStringResult = await itemResponse.Content.ReadAsStringAsync();
                        var listStringResult = await listsResponse.Content.ReadAsStringAsync();

                        ToDo ToDoThing = JsonConvert.DeserializeObject<ToDo>(itemStringResult);
                        List<ToDoList> ListTodoList = JsonConvert.DeserializeObject<List<ToDoList>>(listStringResult);

                        var CorrectList = ListTodoList.Where(l => l.ID == ToDoThing.ListID);
                        ViewData["ToDoList"] = CorrectList;
                        return View(ToDoThing);
                    }
                }
                catch
                {
                    return NotFound();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// GET: TodoItem/Create
        /// </summary>
        /// <returns>view</returns>
        public async Task<IActionResult> Create()
        {
            using (var client = new HttpClient())
            {
                // add the appropriate properties on top of the client base address.
                client.BaseAddress = new Uri("http://lab17-webapi5565.azurewebsites.net/");

                //the .Result is important for us to extract the result of the response from the call
                var response = client.GetAsync($"/api/todolist/").Result;
                if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    var stringResult = await response.Content.ReadAsStringAsync();
                    List<ToDoList> listOfLists = JsonConvert.DeserializeObject<List<ToDoList>>(stringResult);

                    ViewData["TodoList"] = listOfLists ;
                }
            }
            return View();
        }

        /// <summary>
        /// POST: TodoItem/Create
        /// </summary>
        /// <param name="item">TodoItem object to add</param>
        /// <returns>view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Finished,ListID")] ToDo todo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        // add the appropriate properties on top of the client base address.
                        client.BaseAddress = new Uri("http://lab17-webapi5565.azurewebsites.net/");

                        //the .Result is important for us to extract the result of the response from the call
                        var response = await client.PostAsJsonAsync($"/api/todos/", todo);
                    }
                }
                catch
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }
        /// <summary>
        /// GET: Get an item to update
        /// </summary>
        /// <param name="id">the ID # to Update</param>
        /// <returns>view</returns>
        public async Task<IActionResult> Update(int? id)
        {
            using (var client = new HttpClient())
            {
                // add the appropriate properties on top of the client base address.
                client.BaseAddress = new Uri("http://lab17-webapi5565.azurewebsites.net/");

                //the .Result is important for us to extract the result of the response from the call
                var itemResponse = client.GetAsync($"/api/todos/{id}").Result;
                var listsResponse = client.GetAsync($"/api/todolist/").Result;

                if (itemResponse.EnsureSuccessStatusCode().IsSuccessStatusCode
                    && listsResponse.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    var itemStringResult = await itemResponse.Content.ReadAsStringAsync();
                    var listStringResult = await listsResponse.Content.ReadAsStringAsync();

                    ToDo TodoThing = JsonConvert.DeserializeObject<ToDo>(itemStringResult);
                    List<ToDoList> ListofLists = JsonConvert.DeserializeObject<List<ToDoList>>(listStringResult);

                    ViewData["ToDoList"] = ListofLists;
                    return View(TodoThing);
                }
                return View();
            }
        }

        /// <summary>
        /// Put, Updates a selected ToDo
        /// </summary>
        /// <param name="id">the ID # of the item to Update</param>
        /// <param name="item">the item with changes</param>
        /// <returns>view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("ID,Title,Finished,ListID")] ToDo todo)
        {
            if (id != todo.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        // add the appropriate properties on top of the client base address.
                        client.BaseAddress = new Uri("http://lab17-webapi5565.azurewebsites.net/");

                        //the .Result is important for us to extract the result of the response from the call
                        var response = await client.PutAsJsonAsync($"/api/todos/{id}", todo);
                    }
                }
                catch
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        /// <summary>
        /// GET: Delete/TodoItem/id#
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            using (var client = new HttpClient())
            {

                try
                {
                    // add the appropriate properties on top of the client base address.
                    client.BaseAddress = new Uri("http://lab17-webapi5565.azurewebsites.net/");

                    //the .Result is important for us to extract the result of the response from the call
                    var response = client.GetAsync($"/api/todos/{id}").Result;
                    if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                    {
                        var stringResult = await response.Content.ReadAsStringAsync();
                        ToDo ToDoThing = JsonConvert.DeserializeObject<ToDo>(stringResult);

                        return View(ToDoThing);
                    }
                }
                catch
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            using (var client = new HttpClient())
            {
                // add the appropriate properties on top of the client base address.
                client.BaseAddress = new Uri("http://lab17-webapi5565.azurewebsites.net/");

                //the .Result is important for us to extract the result of the response from the call
                var response = await client.DeleteAsync($"/api/todos/{id}");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
