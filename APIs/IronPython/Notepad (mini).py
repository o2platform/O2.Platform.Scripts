import clr
clr.AddReferenceToFileAndPath("_temp\FluentSharp.CoreLib.dll")
clr.AddReferenceToFileAndPath("_temp\FluentSharp.BCL.dll")
import O2
clr.ImportExtensions(O2.DotNetWrappers.ExtensionMethods)

topPanel = "Notepad (mini).py".popupWindow()
textArea = topPanel.add_TextArea();

def openFile(file):	
	textArea.set_Text(file.fileContents().fixCRLF());	

def openFile_AskUser():	
	file = topPanel.askUserForFileToOpen()
	openFile(file)

fileMenu = textArea.mainMenu().add_Menu("File")
fileMenu.add_MenuItem("Open", openFile_AskUser)