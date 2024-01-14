#include "WindowsUtils.h"
#define MIN_ALL 419

WindowsUtils::WindowsUtils()
{
}
WindowsUtils::~WindowsUtils()
{
}

String^ WindowsUtils::GetWallPaperPath()
{
	wchar_t wallpaperPath[MAX_PATH];
	if (SystemParametersInfo(SPI_GETDESKWALLPAPER, MAX_PATH, wallpaperPath, 0))
	{
		return marshal_as<String^>(wallpaperPath);
	}
	else return "";
}
bool WindowsUtils::SetWallpaperByPath(String^ path)
{
	const wchar_t* nativePath = (const wchar_t*)(Marshal::StringToHGlobalUni(path)).ToPointer();
	try
	{
		return SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, (PVOID)nativePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
	}
	finally
	{
		if (nativePath != nullptr)
		{
			Marshal::FreeHGlobal(IntPtr((void*)nativePath));
		}
	}
}
void WindowsUtils::MinimizeAllWindowsByWinD()
{
	//模拟按下Win+D
	keybd_event(VK_LWIN, 0, 0, 0);
	keybd_event('D', 0, 0, 0);
	keybd_event('D', 0, KEYEVENTF_KEYUP, 0);
	keybd_event(VK_LWIN, 0, KEYEVENTF_KEYUP, 0);
}
void WindowsUtils::MinimizeAllWindows()
{
	//最小化所有窗口
	HWND lHwnd = FindWindow(L"Shell_TrayWnd", NULL);
	SendMessage(lHwnd, WM_COMMAND, MIN_ALL, 0);
}

void WindowsUtils::HideTaskBar()
{

	HWND task;
	task = FindWindow(L"Shell_TrayWnd", NULL);
	ShowWindow(task, SW_HIDE);//隐藏任务栏
}
void WindowsUtils::ShowTaskBar()
{

	HWND task;
	task = FindWindow(L"Shell_TrayWnd", NULL);
	ShowWindow(task, SW_SHOW);//显示
}
bool WindowsUtils::IsTaskBarVisible()
{
	HWND task = FindWindow(L"Shell_TrayWnd", NULL);
	bool isVisible = IsWindowVisible(task);
	return isVisible;
}

void WindowsUtils::HideDesktop()
{
	HWND desktop;
	desktop = FindWindow(L"ProgMan", NULL);
	ShowWindow(desktop, SW_HIDE);//隐藏桌面
}
void WindowsUtils::ShowDesktop()
{
	HWND desktop;
	desktop = FindWindow(L"ProgMan", NULL);
	ShowWindow(desktop, SW_SHOW);//显示桌面
}
bool WindowsUtils::IsDesktopVisible()
{
	HWND task = FindWindow(L"ProgMan", NULL);
	bool isVisible = IsWindowVisible(task);
	return isVisible;
}

void WindowsUtils::SwitchDesktopIconsVisible()
{
	HWND hWndPm, hWndPm2, hWndDeskop;
	hWndPm = FindWindowA(NULL, "Program Manager");  //获取桌面句柄
	if (hWndPm != 0)
	{
		hWndPm2 = FindWindowExA(hWndPm, 0, NULL, "");  //获取 hWndPm 的子句柄
		if (hWndPm2 != 0)
		{
			hWndDeskop = FindWindowExA(hWndPm2, 0, NULL, "FolderView");  //获取 hWndPm2 的子句柄
			if (hWndDeskop != 0)
			{
				if (IsWindowVisible(hWndDeskop))
				{
					ShowWindow(hWndDeskop, SW_HIDE);
				}
				else
				{
					ShowWindow(hWndDeskop, SW_SHOW);
				}
			}
		}
	}
}
void WindowsUtils::ShowDesktopIcons()
{
	HWND hWndPm, hWndPm2, hWndDeskop;
	hWndPm = FindWindowA(NULL, "Program Manager");  //获取桌面句柄
	if (hWndPm != 0)
	{
		hWndPm2 = FindWindowExA(hWndPm, 0, NULL, "");  //获取 hWndPm 的子句柄
		if (hWndPm2 != 0)
		{
			hWndDeskop = FindWindowExA(hWndPm2, 0, NULL, "FolderView");  //获取 hWndPm2 的子句柄
			if (hWndDeskop != 0)
			{
				ShowWindow(hWndDeskop, SW_SHOW);
			}
		}
	}
}
void WindowsUtils::HideDesktopIcons()
{
	HWND hWndPm, hWndPm2, hWndDeskop;
	hWndPm = FindWindowA(NULL, "Program Manager");  //获取桌面句柄
	if (hWndPm != 0)
	{
		hWndPm2 = FindWindowExA(hWndPm, 0, NULL, "");  //获取 hWndPm 的子句柄
		if (hWndPm2 != 0)
		{
			hWndDeskop = FindWindowExA(hWndPm2, 0, NULL, "FolderView");  //获取 hWndPm2 的子句柄
			if (hWndDeskop != 0)
			{
				ShowWindow(hWndDeskop, SW_HIDE);
			}
		}
	}
}