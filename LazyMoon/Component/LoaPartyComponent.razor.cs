using LazyMoon.Class.Loa;
using LazyMoon.Class.Service;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace LazyMoon.Component;
public partial class LoaPartyComponent
{
    MudDropContainer<DropItem>? _container;
    bool enabledDDDD;
    bool enabledDDDS;
    bool enabledDDSS;
    bool enabledDSSS;
    bool enabledSDDD;
    bool enabledSDDS;
    bool enabledSDSS;
    bool enabledSSSS;
    public string ResultValue { get; set; } = string.Empty;

    [Inject]
    private ClipboardService? ClipboardService { get; set; }



    [Parameter]
    public int Order { get; set; }

    [Parameter]
    public bool IsRaid8 { get; set; }

    bool _isEnglish;
    [Parameter]
    public bool IsEnglish
    {
        get => _isEnglish;
        set
        {
            if (_isEnglish != value)
            {
                _isEnglish = value;
                Update();
            }
        }
    }

    public LoaParty LoaParty { get; private set; } = new LoaParty();

    private readonly List<DropItem> _items = [];

    public class DropItem
    {
        public Class.Loa.Player? Player { get; init; }
        public string Identifier { get; set; } = string.Empty;
    }

    //private async void Test()
    //{
    //    for (int i = 0; i < 500; i++)
    //    {
    //        ClearButtonClick();
    //        Random rd = new();
    //        var values = Enum.GetValues(typeof(EPartyCombinations));

    //        Random random = new Random();

    //        // ·£´ý ÀÎµ¦½º ¼±ÅÃ
    //        while (LoaParty.MaxCombinations != LoaParty.CurrnetCombinations)
    //        {
    //            if ((values.GetValue(random.Next(values.Length))) is EPartyCombinations randomValue)
    //            {
    //                if (LoaParty.OKPartyCombinations(randomValue))
    //                {
    //                    var playerObject = LoaParty.Add(randomValue);
    //                    _items.Add(new DropItem() { Player = playerObject, Identifier = "Item" });
    //                }
    //            }
    //        }
    //        Update();
    //        MakeButtonClick();
    //        await Task.Delay(1);
    //    }
    //}

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            LoaParty.IsRaid8 = IsRaid8;
            LoaParty.DepartureOrder = Order;
            Update();
        }
        base.OnAfterRender(firstRender);
    }

    private void ItemUpdated(MudItemDropInfo<DropItem> dropItem)
    {
        if (dropItem == null || dropItem.Item == null || dropItem.Item.Player == null)
        {
            return;
        }

        if (dropItem.DropzoneIdentifier.ToLower().Equals("delete", StringComparison.CurrentCultureIgnoreCase))
        {
            LoaParty.Remove(dropItem.Item.Player);
            _items.Remove(dropItem.Item);
        }
        else
        {
            var index = _items.IndexOf(dropItem.Item);
            if (index != dropItem.IndexInZone)
            {
                if (index < dropItem.IndexInZone)
                {
                    _items.Insert(dropItem.IndexInZone + 1, dropItem.Item ?? new DropItem());
                    if (dropItem.Item != null)
                    {
                        var findfIndex = _items.IndexOf(dropItem.Item);
                        _items.RemoveAt(findfIndex);
                    }
                }
                else
                {
                    _items.Insert(dropItem.IndexInZone, dropItem.Item ?? new DropItem());
                    if (dropItem.Item != null)
                    {
                        var findfIndex = _items.LastIndexOf(dropItem.Item);
                        _items.RemoveAt(findfIndex);
                    }
                }
            }
        }
        Update();

    }

    private void OnClick(string player)
    {
        if (Enum.TryParse<EPartyCombinations>(player, out EPartyCombinations partyCombinations))
        {
            if (LoaParty.OKPartyCombinations(partyCombinations))
            {
                var playerObject = LoaParty.Add(partyCombinations);
                _items.Add(new DropItem() { Player = playerObject, Identifier = "Item" });
            }
        }
        Update();
    }

    private void ClearButtonClick()
    {
        foreach (var item in _items)
        {
            if (item.Player != null)
            {
                LoaParty.Remove(item.Player);
            }
        }
        LoaParty.Players.Clear();
        _items.Clear();
        Update();
    }

    private void MakeButtonClick()
    {
        Update();
        ResultValue = IsEnglish ? LoaParty.Make().Replace("¤§", "D").Replace("¤½", "S") : LoaParty.Make();
    }

    private async Task CopyButtonClick()
    {
        await ClipboardService!.WriteTextAsync(ResultValue);
    }

    public async void Update()
    {
        LoaParty.IsRaid8 = IsRaid8;

        foreach (var item in _items)
        {
            if (item.Player != null)
            {
                LoaParty.Players.Remove(item.Player);
                LoaParty.Players.Add(item.Player);
            }
        }

        foreach (EPartyCombinations item in Enum.GetValues(typeof(EPartyCombinations)))
        {
            switch (item)
            {
                case EPartyCombinations.DDDD:
                    enabledDDDD = !LoaParty.OKPartyCombinations(item);
                    break;
                case EPartyCombinations.DDDS:
                    enabledDDDS = !LoaParty.OKPartyCombinations(item);
                    break;
                case EPartyCombinations.DDSS:
                    enabledDDSS = !LoaParty.OKPartyCombinations(item);
                    break;
                case EPartyCombinations.DSSS:
                    enabledDSSS = !LoaParty.OKPartyCombinations(item);
                    break;
                case EPartyCombinations.SDDD:
                    enabledSDDD = !LoaParty.OKPartyCombinations(item);
                    break;
                case EPartyCombinations.SDDS:
                    enabledSDDS = !LoaParty.OKPartyCombinations(item);
                    break;
                case EPartyCombinations.SDSS:
                    enabledSDSS = !LoaParty.OKPartyCombinations(item);
                    break;
                case EPartyCombinations.SSSS:
                    enabledSSSS = !LoaParty.OKPartyCombinations(item);
                    break;
                default:
                    break;
            }
        }

        await InvokeAsync(() =>
        {
            StateHasChanged(); _container?.Refresh();
        });
    }
}