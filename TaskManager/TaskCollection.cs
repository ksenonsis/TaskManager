using System;
using System.Collections.Generic;
using System.Linq;

namespace TaskManager
{
    // Класс для управления списком задач
    public class TaskCollection
    {
        private List<TaskItem> _tasks = new List<TaskItem>();
        private int _nextId = 1;

        // Добавить задачу
        public void Add(TaskItem task)
        {
            task.Id = _nextId++;
            _tasks.Add(task);
        }

        // Удалить задачу по ID
        public bool Remove(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);
            if (task != null)
            {
                _tasks.Remove(task);
                return true;
            }
            return false;
        }

        // Получить все задачи
        public List<TaskItem> GetAll()
        {
            return new List<TaskItem>(_tasks);
        }

        // Получить задачи по статусу
        public List<TaskItem> GetByStatus(TaskStatus status)
        {
            return _tasks.Where(t => t.Status == status).ToList();
        }

        // Поиск по названию
        public List<TaskItem> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return GetAll();

            return _tasks.Where(t =>
                t.Title.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                t.Description.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0
            ).ToList();
        }

        // Найти задачу по ID
        public TaskItem GetById(int id)
        {
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        // Количество задач
        public int Count => _tasks.Count;
    }
}
