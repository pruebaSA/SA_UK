// (c) 2010 CodePlex Foundation
Type.registerNamespace("Sys.Extended.UI.Seadragon");Sys.Extended.UI.Seadragon.TileSource=function(c,b,g,d,f,e){var a=this;a.aspectRatio=c/b;a.dimensions=new Sys.Extended.UI.Seadragon.Point(c,b);a.minLevel=f?f:0;a.maxLevel=e?e:Math.ceil(Math.log(Math.max(c,b))/Math.log(2));a.tileSize=g?g:0;a.tileOverlap=d?d:0};Sys.Extended.UI.Seadragon.TileSource.prototype={getLevelScale:function(a){return 1/(1<<this.maxLevel-a)},getNumTiles:function(c){var a=this,b=a.getLevelScale(c),d=Math.ceil(b*a.dimensions.x/a.tileSize),e=Math.ceil(b*a.dimensions.y/a.tileSize);return new Sys.Extended.UI.Seadragon.Point(d,e)},getPixelRatio:function(b){var a=this.dimensions.times(this.getLevelScale(b)),c=1/a.x,d=1/a.y;return new Sys.Extended.UI.Seadragon.Point(c,d)},getTileAtPoint:function(c,d){var a=this,b=d.times(a.dimensions.x).times(a.getLevelScale(c)),e=Math.floor(b.x/a.tileSize),f=Math.floor(b.y/a.tileSize);return new Sys.Extended.UI.Seadragon.Point(e,f)},getTileBounds:function(j,f,g){var a=this,c=a.dimensions.times(a.getLevelScale(j)),h=f===0?0:a.tileSize*f-a.tileOverlap,i=g===0?0:a.tileSize*g-a.tileOverlap,d=a.tileSize+(f===0?1:2)*a.tileOverlap,e=a.tileSize+(g===0?1:2)*a.tileOverlap;d=Math.min(d,c.x-h);e=Math.min(e,c.y-i);var b=1/c.x;return new Sys.Extended.UI.Seadragon.Rect(h*b,i*b,d*b,e*b)},getTileUrl:function(){throw new Error("Method not implemented.");},tileExists:function(a,c,d){var b=this.getNumTiles(a);return a>=this.minLevel&&a<=this.maxLevel&&c>=0&&d>=0&&c<b.x&&d<b.y}};Sys.Extended.UI.Seadragon.TileSource.registerClass("Sys.Extended.UI.Seadragon.TileSource",null,Sys.IDisposable);