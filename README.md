# RegX
[![Build status](https://ci.appveyor.com/api/projects/status/jb2s8gx5n2ctb7xh?svg=true)](https://ci.appveyor.com/project/MathewSachin/regx)  
A Regex Parser Tool for instantly checking your Regex

Examples
---------------------------------------
Email
(?("")("".+?""@)|(([0-9a-zA-Z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-zA-Z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]\.)+[a-zA-Z]{2,6}))

URL
(http|ftp|https):\/\/[\w\-_]+(\.[\w\-_]+)+([\w\-\.,@?^=%&amp;:/~\+#]*[\w\-\@?^=%&amp;/~\+#])?
