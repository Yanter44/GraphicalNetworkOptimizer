using System;
using System.Collections.Generic;
using System.Text;

namespace NetOptimizer.Models.Enums
{
    public enum TopologyType
    {
        SingleStar,       // Один свитч (до 24-48 ПК)
        HierarchicalTree, // Дерево (Ядро + Доступ, для твоих 140 ПК)
        Ring,             // Кольцо (для экономии кабеля/длинных дистанций)
        DaisyChain       // Гирлянда (бюджетный вариант, один в другой)
    }
}
