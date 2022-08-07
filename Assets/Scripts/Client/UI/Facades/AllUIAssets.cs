using System.Threading.Tasks;
using Game.UI.Assets;

namespace Game.UI.Facades
{

    public static class AllUIAssets
    {
        public static UIAssets UIAssets { get; private set; }
        public static void Ctor()
        {
            UIAssets = new UIAssets();
        }

        public static async Task LoadAll()
        {
            await UIAssets.LoadAll();
        }
    }

}