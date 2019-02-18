using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToolModXdLib.Core;
using ToolModXdLib.Models;

namespace ToolModXdLib
{
    public delegate void InjectorMsgHandler(string msg);

    internal interface IVersionInjector
    {
        event InjectorMsgHandler EventMessanger;

        /// <summary>
        /// Получает ячейку для редактирования в GUI
        /// </summary>
        /// <returns></returns>
        List<CellEditor> GetCellsEditor();

        /// <summary>
        /// Читает файл построчно
        /// </summary>
        void Read(string filePath);

        /// <summary>
        /// Превращает прочтенный файл в список объектов, с которым дальше программа будет работать
        /// </summary>
        /// <param name="IsLoadGameplayData">Поднимать ли данные, которые влияют на игровой процесс</param>
        void Objectivation(bool IsLoadGameplayData);

        /// <summary>
        /// Загрузка и объективация файла, в который необходимо перекинуть изменения
        /// </summary>
        /// <param name="filePath">Целевой файл</param>
        void LoadTarget(string filePath);

        /// <summary>
        /// Запуск процедуры миграции изменений
        /// </summary>
        void Inject();

        /// <summary>
        /// Сохранить результат
        /// </summary>
        /// <param name="dirPath"></param>
        void SaveResult(string dirPath);

        // other
        /// <summary>
        /// Получить список данных для listFile путем обработки списка данных, полученного путем Objectivation
        /// </summary>
        /// <param name="injector">Лист в который необходимо добавить найденные данные</param>
        void GetDataForListfile(ListFileInjector injector);
    }
}
