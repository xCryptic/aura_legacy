using World.Scripting;

public class ConnousPortals : BaseScript
{
	public override void OnLoad()
	{
		DefineProp(45049323557289986, 3103, 1199, 1267, PropAction.Warp, 3100, 350551, 415985);
		DefineProp(45049332147421193, 3105, 28061, 18017, PropAction.Warp, 3100, 142665, 323377);
		DefineProp(45036485900107783, 114, 1137, 1, PropAction.Warp, 3100, 368304, 428944);
		DefineProp(45049310685364308, 3100, 350228, 416003, PropAction.Warp, 3103, 1193, 1018);
		DefineProp(45049310686085166, 3100, 142188, 323449, PropAction.Warp, 3105, 27569, 18107);
		DefineProp(45049310686085166, 3100, 377320, 431815, PropAction.Warp, 3106, 3141, 2922);
		DefineProp(45049310685364308, 3106, 3141, 2922, PropAction.Warp, 3100, 377320, 431815);
	}
}
