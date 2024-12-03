# Лабораторная работа на тему "HTTP"

## Был реализован проект "TO DO list"

# TaskItem Class

Класс `TaskItem` представляет собой модель элемента задачи, используемую для управления задачами в приложении. Он содержит информацию о задаче, такую как идентификатор, заголовок, описание, статус, приоритет и срок выполнения.

## Свойства

- `Id` (int): Уникальный идентификатор задачи.
- `Title` (string): Заголовок задачи. По умолчанию — пустая строка.
- `Description` (string): Описание задачи. По умолчанию — пустая строка.
- `Status` (string): Текущий статус задачи. По умолчанию — "Новая".
- `Priority` (string): Приоритет задачи. По умолчанию — "Средний".
- `DueDate` (DateTime): Дата и время, к которым задача должна быть выполнена.

## Конструкторы

Класс `TaskItem` имеет два конструктора:

1. **Без параметров**: Инициализирует новый экземпляр класса `TaskItem` с значениями по умолчанию.
   
   ```csharp
   public TaskItem()
   {
   }
   ```
2. **С параметрами:** Позволяет создать экземпляр класса с заданными значениями для всех свойств.
   ```csharp
    public TaskItem(int id, string title, string description, string status, string priority, DateTime dueDate)
    {
    Id = id;
    Title = title;
    Description = description;
    Status = status;
    Priority = priority;
    DueDate = dueDate;
    }
   ```

 Конечно! Давайте разберем данный код, который представляет собой реализацию клиента API для работы с задачами. Этот класс называется `TaskApiClient` и реализует интерфейс `ITaskApi`. Он использует `HttpClient` для выполнения HTTP-запросов к API.

## Структура класса

### Поля и Конструктор

```csharp
private readonly HttpClient httpClient;

public TaskApiClient(HttpClient httpClient)
{
    this.httpClient = httpClient;
}
```

- **Поле `httpClient`**: Это поле хранит экземпляр `HttpClient`, который используется для выполнения HTTP-запросов.
- **Конструктор**: Конструктор принимает объект `HttpClient` в качестве параметра и инициализирует поле `httpClient`. Это позволяет использовать один и тот же экземпляр `HttpClient` для всех методов класса, что является хорошей практикой для оптимизации производительности.

## Методы

### 1. Добавление задачи

```csharp
public async Task<bool> AddAsync(TaskItem newTask)
{
    var response = await httpClient.PostAsJsonAsync("Task", newTask);
    return response.IsSuccessStatusCode;
}
```

- **`AddAsync`**: Этот метод добавляет новую задачу, отправляя POST-запрос с данными задачи (`newTask`) в формате JSON. Если запрос успешен (код состояния 2xx), метод возвращает `true`, в противном случае — `false`.

### 2. Удаление задачи

```csharp
public async Task<bool> DeleteAsync(int id)
{
    var response = await httpClient.DeleteAsync($"Task/{id}");
    return response.IsSuccessStatusCode;
}
```

- **`DeleteAsync`**: Метод удаляет задачу по идентификатору (`id`). Он отправляет DELETE-запрос на указанный URL. Возвращает `true`, если операция успешна, и `false` в противном случае.

### 3. Получение всех задач

```csharp
public async Task<TaskItem[]> GetAllAsync()
{
    var results = await httpClient.GetFromJsonAsync<TaskItem[]>("Task");
    return results?.ToArray() ?? Array.Empty<TaskItem>();
}
```

- **`GetAllAsync`**: Этот метод получает все задачи, отправляя GET-запрос на URL "Task". Он возвращает массив объектов `TaskItem`. Если запрос не возвращает результатов, возвращается пустой массив.

### 4. Получение задачи по идентификатору

```csharp
public async Task<TaskItem?> GetAsync(int id)
{
    try
    {
        var result = await httpClient.GetFromJsonAsync<TaskItem?>($"Task/{id}");
        return result;
    }
    catch
    {
        return null;
    }
}
```

- **`GetAsync`**: Метод получает конкретную задачу по идентификатору (`id`). Если запрос успешен, он возвращает объект `TaskItem`. В случае ошибки (например, если задача не найдена) метод возвращает `null`.

### 5. Обновление задачи

```csharp
public async Task<bool> UpdateAsync(int id, TaskItem updateTask)
{
    var response = await httpClient.PutAsJsonAsync($"Task/{id}", updateTask);
    return response.IsSuccessStatusCode;
}
```

- **`UpdateAsync`**: Этот метод обновляет существующую задачу, отправляя PUT-запрос с обновленными данными (`updateTask`). Возвращает `true`, если операция успешна, и `false`, если нет.
Конечно! Давайте кратко разберем код класса `TaskStorage`, который реализует хранилище для объектов типа `TaskItem` и интерфейс `ITaskApi`. Этот класс управляет задачами, используя внутреннее хранилище на основе `ConcurrentDictionary`.

## Основные компоненты класса TaskStorage

### 1. Поля класса

- **`defaultData`**: Статический словарь, содержащий несколько предустановленных задач. Эти задачи используются для инициализации хранилища, если в нем нет сохраненных данных.
  
- **`taskRepository`**: Статический экземпляр `ConcurrentDictionary`, который служит основным хранилищем для задач. Он обеспечивает потокобезопасный доступ к данным.

- **`_lastId`**: Статическое поле для хранения последнего использованного идентификатора задачи. Оно используется для генерации новых уникальных идентификаторов.

### 2. Конструктор

```csharp
public TaskStorage(IDataSerializer<TaskItem[]> dataSerializer)
```

Конструктор принимает объект сериализатора данных (`IDataSerializer<TaskItem[]>`) и инициализирует базовый класс `StorageBase<TaskItem>`, задавая путь к файлу для хранения данных. Внутри конструктора происходит чтение существующих данных из файла. Если данные отсутствуют, используется `defaultData` для инициализации `taskRepository`.

### 3. Методы

#### 3.1 Добавление задачи

```csharp
public async Task<bool> AddAsync(TaskItem newTask)
```

- Метод добавляет новую задачу в хранилище. Если задача с таким идентификатором уже существует, возвращает `false`. В противном случае задача добавляется, и данные записываются обратно в файл.

#### 3.2 Удаление задачи

```csharp
public async Task<bool> DeleteAsync(int id)
```

- Метод удаляет задачу по идентификатору. Если задача не найдена, возвращает `false`. После удаления данные снова записываются в файл.

#### 3.3 Получение всех задач

```csharp
public Task<TaskItem[]> GetAllAsync()
```

- Метод возвращает массив всех задач из хранилища.

#### 3.4 Получение задачи по идентификатору

```csharp
public Task<TaskItem?> GetAsync(int id)
```

- Метод возвращает задачу по указанному идентификатору. Если задача не найдена, возвращает `null`.

#### 3.5 Обновление задачи

```csharp
public async Task<bool> UpdateAsync(int id, TaskItem updateTask)
```

- Метод обновляет существующую задачу по идентификатору. Если задача не найдена, возвращает `false`. После обновления данные записываются в файл.

## Работа программы
![](https://github.com/sorrymorning/httpToDoList/blob/master/firstImage.png)
![](https://github.com/sorrymorning/httpToDoList/blob/master/SecondImage.png)
