using UnityEngine;

// Thin delegate — navigation is now owned by NavController.
// Keeping this class alive so any existing Inspector button wiring still compiles.
public class GameStateController : MonoBehaviour
{
    public void FarmContext()    => NavController.Instance.Navigate(NavController.TAB_FARM);
    public void OpenEgg()        => NavController.Instance.Navigate(NavController.TAB_EGGS);
    public void EconomyContext() => NavController.Instance.Navigate(NavController.TAB_UPGRADES);
    public void BarnContext()    => NavController.Instance.Navigate(NavController.TAB_BARN);
}
