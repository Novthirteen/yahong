Type.registerNamespace("AjaxControlToolkit.HTMLEditor");AjaxControlToolkit.HTMLEditor.Toolbar=function(b){var a=this;AjaxControlToolkit.HTMLEditor.Toolbar.initializeBase(a,[b]);a._loaded=false;a._cachedButtonIds=null;a._cachedEditPanel=null;a._buttons=null;a._alwaysVisible=false;a._app_onload$delegate=Function.createDelegate(a,a._app_onload)};AjaxControlToolkit.HTMLEditor.Toolbar.prototype={get_alwaysVisible:function(){return this._alwaysVisible},set_alwaysVisible:function(a){this._alwaysVisible=a;this.get_isInitialized()&&this.raisePropertyChanged("alwaysVisible")},set_activeEditPanel:function(c){var a=this;if(!a._loaded){a._cachedEditPanel=c;return}for(var b=0;b<a.get_buttons().length;b++)a.get_buttons()[b].set_activeEditPanel(c)},disable:function(){var a=this;if(a.get_isInitialized()){if(a._alwaysVisible)return;for(var b=0;b<a.get_buttons().length;b++)a.get_buttons()[b].set_activeEditPanel(null)}},get_buttons:function(){if(this._buttons==null)this._buttons=[];return this._buttons},set_buttons:function(a){this.get_buttons().push(a)},get_buttonIds:function(){},set_buttonIds:function(c){if(!this.get_isInitialized()){this._cachedButtonIds=c;return}for(var b=c.split(";"),a=0;a<b.length;a++)b[a].length>0&&this.set_buttons($find(b[a]))},initialize:function(){AjaxControlToolkit.HTMLEditor.Toolbar.callBaseMethod(this,"initialize");Sys.Application.add_load(this._app_onload$delegate)},dispose:function(){this._loaded=false;Sys.Application.remove_load(this._app_onload$delegate);AjaxControlToolkit.HTMLEditor.Toolbar.callBaseMethod(this,"dispose")},_app_onload:function(){var b=null,a=this;if(a._cachedButtonIds!=b){a.set_buttonIds(a._cachedButtonIds);a._cachedButtonIds=b}a._loaded=true;if(a._cachedEditPanel!=b){a.set_activeEditPanel(a._cachedEditPanel);a._cachedEditPanel=b}}};AjaxControlToolkit.HTMLEditor.Toolbar.registerClass("AjaxControlToolkit.HTMLEditor.Toolbar",Sys.UI.Control);