# PTGui Language Editor

The PTGui Language Editor simplifies editing PTGui languages files. Instead of editing the Json files direcly the PTGui Language Editor offers a convenient user interface. You see both the English text and your edited text in parallel. Additionally the PTGui Language Editor gives you a formatted preview of the English text and a real-time preview of your edited (raw) text.

After opening the editor, click the 'Load' button and browse to the folder containing the .nhloc files. In general this folder will be the folder of your fork of the PTGui language repository and not the folder of the PTGui installation. Then select the language in the dropdown (e.g. 'de_de').

The PTGui Language Editor shows three columns (the 'General' tab shows only two columns):

+ The first column shows the formatted preview of the English text.
+ The second column shows the formatted preview of the selected language
+ The third column contains the raw text (with all the control codes) of your selected language, this is the column where you make your changes.

Every keystroke updates the preview in the second column.

You still enter all the control codes as described in the PTGui language project. In order to simplify editing there is one single exception: you don't need to take care of the line feed control code '\<CR>'. For the time of editing '\<CR>' is replaced by a real line feed in the third column which makes the raw text better readable. You insert a line feed by just pressing the ENTER key. The real line feeds are automatically converted back to '\<CR>' when you save the file.

The formatted preview formats:

+ replace string lookups with the final text (like @ok@)
+ bold text
+ hyperlinks
+ &amp;amp;
+ red text
+ conditional text on Windows only is displayed with cyan background
+ conditional text on Max only is displayed with cyan green background
+ conditional text for the Pro version only is displayed with red background


![intro](/Tooltips.png)

The language editor requires a .Net 7 runtime installation.