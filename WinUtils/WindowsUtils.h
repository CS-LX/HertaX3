#pragma once
#include <windows.h>
#include <WinUser.h>
#include <msclr/marshal_cppstd.h>
#include <msclr/marshal.h>

using namespace System;
using namespace System::Runtime::InteropServices;
using namespace msclr::interop;

public ref class WindowsUtils
{
public:
	WindowsUtils();
	~WindowsUtils();
	String^ GetWallPaperPath();
	bool SetWallpaperByPath(String^ path);
	void MinimizeAllWindowsByWinD();
	void MinimizeAllWindows();
	void HideTaskBar();
	void ShowTaskBar();
	bool IsTaskBarVisible();
	void HideDesktop();
	void ShowDesktop();
	bool IsDesktopVisible();
	void SwitchDesktopIconsVisible();
	void ShowDesktopIcons();
	void HideDesktopIcons();
};
