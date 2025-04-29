using System.Collections.Generic;

namespace Umbra.Game;

public interface IMainMenuRepository
{
    public List<MainMenuCategory> GetCategories();

    public MainMenuCategory GetCategory(MenuCategory category);

    public MainMenuItem? FindById(string id);
}
