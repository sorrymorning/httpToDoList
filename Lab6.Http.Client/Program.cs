using Lab6.Http.Common;

internal class Program
{
    private static object _locker = new object();

    public static async Task Main(string[] args)
    {
        var httpClient = new HttpClient()
        {
            BaseAddress = new Uri("http://localhost:5214/api/")
        };

        var taskApiClient = new TaskApiClient(httpClient);

        await ManageTasks(taskApiClient);
    }

    private static async Task ManageTasks(ITaskApi taskApi)
    {
        PrintMenu();

        while (true)
        {
            var key = Console.ReadKey(true);

            PrintMenu();

            if (key.Key == ConsoleKey.D1)
            {
                var tasks = await taskApi.GetAllAsync();
                Console.WriteLine($"| Id    |   Title        | Priority | Status  | Due Date   |");
                foreach (var task in tasks)
                {
                    Console.WriteLine($"| {task.Id,5} | {task.Title,15} | {task.Priority,8} | {task.Status,8} | {task.DueDate:yyyy-MM-dd} |");
                }
            }

            if (key.Key == ConsoleKey.D2)
            {
                Console.Write("Введите id задачи: ");
                var taskIdString = Console.ReadLine();
                int.TryParse(taskIdString, out var taskId);
                var task = await taskApi.GetAsync(taskId);
                if (task == null)
                {
                    Console.WriteLine("Задача не найдена.");
                }
                else
                {
                    Console.WriteLine($"Id={task.Id}, Title={task.Title}, Status={task.Status}, DueDate={task.DueDate:yyyy-MM-dd}");
                }
            }

            if (key.Key == ConsoleKey.D3)
            {
                Console.Write("Введите название задачи: ");
                var title = Console.ReadLine() ?? "Без названия";
                Console.Write("Введите описание задачи: ");
                var description = Console.ReadLine() ?? "Без описания";
                Console.Write("Введите приоритет задачи (Низкий/Средний/Высокий): ");
                var priority = Console.ReadLine() ?? "Средний";
                Console.Write("Введите крайний срок задачи (yyyy-MM-dd): ");
                var dueDateInput = Console.ReadLine();
                DateTime.TryParse(dueDateInput, out var dueDate);

                var newTask = new TaskItem(
                    id: 0,
                    title: title,
                    description: description,
                    status: "Новая",
                    priority: priority,
                    dueDate: dueDate == DateTime.MinValue ? DateTime.Now.AddDays(7) : dueDate
                );

                var addResult = await taskApi.AddAsync(newTask);
                Console.WriteLine(addResult ? "Задача добавлена." : "Ошибка добавления задачи.");
            }

            if (key.Key == ConsoleKey.D4)
            {
                Console.Write("Введите id задачи: ");
                var taskIdString = Console.ReadLine();
                int.TryParse(taskIdString, out var taskId);
                var task = await taskApi.GetAsync(taskId);
                if (task == null)
                {
                    Console.WriteLine("Задача не найдена.");
                    continue;
                }

                Console.Write("Введите новый статус задачи: ");
                var newStatus = Console.ReadLine() ?? "Новая";
                task.Status = newStatus;

                var updateResult = await taskApi.UpdateAsync(taskId, task);
                Console.WriteLine(updateResult ? "Задача обновлена." : "Ошибка обновления задачи.");
            }

            if (key.Key == ConsoleKey.D5)
            {
                Console.Write("Введите id задачи: ");
                var taskIdString = Console.ReadLine();
                int.TryParse(taskIdString, out var taskId);

                var deleteResult = await taskApi.DeleteAsync(taskId);
                Console.WriteLine(deleteResult ? "Задача удалена." : "Ошибка удаления задачи.");
            }

            if (key.Key == ConsoleKey.Escape)
            {
                break;
            }
        }
    }

    private static void PrintMenu()
    {
        lock (_locker)
        {
            Console.WriteLine("1 - Показать все задачи");
            Console.WriteLine("2 - Показать задачу по id");
            Console.WriteLine("3 - Добавить задачу");
            Console.WriteLine("4 - Обновить задачу");
            Console.WriteLine("5 - Удалить задачу");
            Console.WriteLine("-------");
        }
    }
}
