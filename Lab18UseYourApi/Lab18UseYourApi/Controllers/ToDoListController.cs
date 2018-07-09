using Lab18UseYourApi.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Lab18WebApp.Controllers
{
    public class TodoListController : Controller
    {
        /// <summary>
        /// GET: TodoList/
        /// </summary>
        /// <param name="searchString">string to search for specific todo list by name</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string searchString)
        {
            using (var client = new HttpClient())
            {
                // add the appropriate properties on top of the client base address.
                client.BaseAddress = new Uri("http://lab17-webapi5565.azurewebsites.net/");

                //the .Result is important for us to extract the result of the response from the call
                var listsResponse = client.GetAsync("/api/ToDoList/").Result;
                var itemsResponse = client.GetAsync("/api/ToDos/").Result;
                if (listsResponse.EnsureSuccessStatusCode().IsSuccessStatusCode
                    && listsResponse.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    var stringListResult = await listsResponse.Content.ReadAsStringAsync();
                    var stringItemsResult = await itemsResponse.Content.ReadAsStringAsync();

                    List<ToDoList> TodoLists = JsonConvert.DeserializeObject<List<ToDoList>>(stringListResult);
                    List<ToDo> Todos = JsonConvert.DeserializeObject<List<ToDo>>(stringItemsResult);

                    var Lists = from l in TodoLists
                                    select l;

                    var ThingsToDo = from i in Todos
                                    select i;

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        Lists = Lists.Where(s => s.Name.Contains(searchString));
                    }

                    var ListVM = new ToDoListViewModel();
                    ListVM.TodoLists = Lists.ToList();
                    ListVM.TodoItems = ThingsToDo.ToList();
                    return View(ListVM);
                }
                return View();
            }
        }

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
                    var listResponse = client.GetAsync($"/api/todolist/{id}").Result;
                    var itemsResponse = client.GetAsync("/api/todos/").Result;

                    if (listResponse.EnsureSuccessStatusCode().IsSuccessStatusCode
                        && itemsResponse.EnsureSuccessStatusCode().IsSuccessStatusCode)
                    {
                        var listStringResult = await listResponse.Content.ReadAsStringAsync();
                        var itemsStringResult = await itemsResponse.Content.ReadAsStringAsync();

                        ToDoList TodoList = JsonConvert.DeserializeObject<ToDoList>(listStringResult);
                        List<ToDo> demTodoItems = JsonConvert.DeserializeObject<List<ToDo>>(itemsStringResult);

                        List<ToDo> CorrectToDos = demTodoItems.Where(i => i.ListID == id).ToList();

                        TodoList.Contents = CorrectToDos;
                        return View(TodoList);
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
        /// GET: TodoList/Create
        /// </summary>
        /// <returns>view</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// POST: TodoList/Create
        /// </summary>
        /// <param name="list">the TodoList object to add</param>
        /// <returns>view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] ToDoList list)
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
                        var response = await client.PostAsJsonAsync($"/api/todolist/", list);
                    }
                }
                catch
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(list);
        }

        /// <summary>
        /// GET: TodoList/Update/id#
        /// </summary>
        /// <param name="id">ID # of the TodoList object to Update</param>
        /// <returns>view</returns>
        public async Task<IActionResult> Update(int? id)
        {
            using (var client = new HttpClient())
            {
                // add the appropriate properties on top of the client base address.
                client.BaseAddress = new Uri("http://lab17-webapi5565.azurewebsites.net/");

                //the .Result is important for us to extract the result of the response from the call
                var response = client.GetAsync($"/api/todolist/{id}").Result;
                if (response.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    var stringResult = await response.Content.ReadAsStringAsync();
                    ToDoList OneList = JsonConvert.DeserializeObject<ToDoList>(stringResult);

                    return View(OneList);
                }
                return View();
            }
        }

        /// <summary>
        /// POST: TodoList/Edit/id#
        /// </summary>
        /// <param name="id">ID # of the TodoList object to edit</param>
        /// <param name="list">a new TodoList object with the edited properties</param>
        /// <returns>view</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, [Bind("ID,Name")] ToDoList list)
        {
            if (id != list.ID)
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
                        var response = await client.PutAsJsonAsync($"/api/todolist/{id}", list);
                    }
                }
                catch
                {
                    return NotFound();
                }
                return RedirectToAction(nameof(Index));
            }
            return View(list);
        }

        /// <summary>
        /// GET: TodoList/Delete/id#
        /// </summary>
        /// <param name="id">ID # of the TodoList object to delete</param>
        /// <returns>view</returns>
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
                    var listResponse = client.GetAsync($"/api/todolist/{id}").Result;
                    var itemsResponse = client.GetAsync("/api/todos/").Result;

                    if (listResponse.EnsureSuccessStatusCode().IsSuccessStatusCode
                        && itemsResponse.EnsureSuccessStatusCode().IsSuccessStatusCode)
                    {
                        var listStringResult = await listResponse.Content.ReadAsStringAsync();
                        var itemsStringResult = await itemsResponse.Content.ReadAsStringAsync();

                        ToDoList listofLists = JsonConvert.DeserializeObject<ToDoList>(listStringResult);
                        List<ToDo> listToDos = JsonConvert.DeserializeObject<List<ToDo>>(itemsStringResult);

                        List<ToDo> correctItems = listToDos.Where(i => i.ListID == id).ToList();

                        listofLists.Contents = correctItems;
                        return View(listofLists);
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
                var itemsResponse = client.GetAsync("/api/todos/").Result;
                if (itemsResponse.EnsureSuccessStatusCode().IsSuccessStatusCode)
                {
                    var stringItemsResult = await itemsResponse.Content.ReadAsStringAsync();

                    List<ToDo> TodoThings = JsonConvert.DeserializeObject<List<ToDo>>(stringItemsResult);

                    var GoneToDo = from i in TodoThings
                                      where i.ListID == id
                                      select i;

                    foreach (var item in GoneToDo)
                    {
                        await client.DeleteAsync($"/api/todos/{item.ID}");
                    }

                    var listResponse = await client.DeleteAsync($"/api/todolist/{id}");
                }
                return RedirectToAction(nameof(Index));
            }
        }
    }
}