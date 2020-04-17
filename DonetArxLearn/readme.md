# Dotnet Arx Learning
这里是我学习Dotnet Arx一些经验分享

## Layouts and Plot
Layout就是CAD中的布局，同时它也是打印相关参数的保存实体。

### Layout获取
Layout是保存在LayoutDictionary中的。可以先通过db.LayoutDictionaryId获得DBDictionary,
然后通过GetAt(layoutName)方法来直接获取到对应id，继而直接得到Layout对象

```js
var layoutDictionary = trans.GetObject(db.LayoutDictionaryId, OpenMode.ForRead) as DBDictionary;
var layout = layoutDictionary.GetAt(layoutName).GetObject(OpenMode.ForRead) as Layout;
```

