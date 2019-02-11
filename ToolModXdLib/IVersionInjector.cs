using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolModXdLib
{
    public delegate void InjectorMsgHandler(string msg);

    public interface IVersionInjector
    {
        event InjectorMsgHandler EventMessanger;

        Task Read(string filePath);

        Task Objectivation(bool IsLoadGameplayData);

        Task Inject(List<object> targetList);

        Task SaveResult(string dirPath);
    }
}
