using System.Threading.Tasks;
using Game.UI.Assets;

namespace Game.Facades
{
    public static class AllAssets
    {

        public static UIAssets UIASSETS;

        public static void Ctor()
        {
            UIASSETS = new UIAssets();
        }

        public static async Task LoadAll()
        {
            await UIASSETS.LoadAll();
        }

    }

}




