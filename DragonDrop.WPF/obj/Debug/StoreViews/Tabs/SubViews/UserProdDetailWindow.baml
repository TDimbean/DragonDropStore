   M S B A M L   `   `   `  ����I  EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews'DragonDrop.WPF.StoreViews.Tabs.SubViews  5   	   e7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters)DragonDrop.WPF.StoreViews.Tabs.Converters  5   	   B    <DragonDrop.WPF.StoreViews.Tabs.SubViews.UserProdDetailWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Glocal5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews     5   	   Hconv7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters     5   	     � Title$  Product Details��5	   	    � WindowStartupLocation$ CenterScreen=�6"    � WindowStyle$ 
ToolWindow=�6G    � 
ResizeMode$ NoResize=�6`    � SizeToContent$ WidthAndHeight=�6v   ���#FFBB00 � 	Resources 5       �� 
TargetType �� Property �� Value   AlternateRowBg	 �� Color  
MainColourE   ?DragonDrop.WPF.StoreViews.Tabs.Converters.IdToCategoryConverter  IdToCatConvE   ?DragonDrop.WPF.StoreViews.Tabs.Converters.RawCodeToUpcConverter  RawCodeToUpcConv��%~  '��       '�� n     &	  �     &	 �     &	     &	 /    �� 5      " ��6   .�5      ��  � ��$ 18(�6-   �� 5       � ��$
 Left=�68   5      �� 5      " ��6   .�5      ��  � �� 6/   Z� �� 5       � �� 60   Z� 5      �� 5      $	 #f5972e��65   �� 5      $	 #F9BB77��61     5      6)     5      6)   5      .��5      �� .+�5   
   ]� $��4q�6   .+�5      �� $-��'pack://application:,,,/Art/products.png��6   $��60��6E   $
��Left=�6Q   $��0q�6g   �� 5      $��Center=�6   $
��Wrap=�6K   $	��260��6_   $��0q�5       
 �� Padding$
 0q�6#   $��24(�6/   ��5   5   �� *Name+5    /   5!      �� 5"   
    �� Orientation$ 
Horizontal=�6   #��[  6.    � ToolTip$ Category��6[   .+�5#      �� $)��#pack://application:,,,/Art/pawn.png��6   $��30��6A   $��4q�6M   �� 5$      $��Center=�5%   .   ��5$      �� *
CategoryId+ �� 	Converter# [ ��5%      Z� 6.   5&      �� 5'   
   $ 
Horizontal=�6   .+�5(      �� $ 
Horizontal=�6   $	��160��62   $ Price��6>   .+�5)      �� $*��$pack://application:,,,/Art/money.png��6   $��4q�6F   $��30��6Q   �� 5*      $��Center=�6G   ��6   �� *	UnitPrice+ �� StringFormat$ c����6b   Z� 5+      �� 5-      $ 
Horizontal=�6   $ Stock��63   .+�5.      �� $*��$pack://application:,,,/Art/store.png��6   $��30��6F   $��4q�6R   �� 5/      $��Center=�6I   ��6   �� *Stock+��63   Z� 6I   50      52      �� 53   
   -   5         ��Name$ DetailsStack��53      $ Vertical=�6+   $y�	Collapsed=�6B    54      ��%U  '��       '�� k     '�� �     �� 56      " ��6   .�57      ��  � ��$ 24��63   �� 58       � ��$ 4q�63   59      �� 5:      " ��6   .�5;      ��  � ��$ Center=�6>   �� 5<       � ��$ 18(�65   5=      �� 5>      " ��6   .�5?      ��  �@��Orientation$ 
Horizontal=�68   5@      5B      .+�5C      �� #��[  6   $ Description��6F   .+�5D      �� $)��#pack://application:,,,/Art/info.png��6   $	��Top=�6E   �� 5E      $
��Wrap=�69   $	��300��6M   ��6   �� *Description+6M   5F      �� 5G      $ Barcode��6   .+�5H      �� $,��&pack://application:,,,/Art/barcode.png��6   �� 5I      ��6   �� *	BarCode+# [ 5J      �� 5K      #��[  6   $ Manufacturer��6F   .+�5L      �� $1��+pack://application:,,,/Art/manufacturer.png��6   �� 5M      ��6   �� *Manufacturer+5N      5O      �� 5P   
   -   5        
���z���.��5Q      �� $ 
Horizontal=�6   .+�5R      �� $-��'pack://application:,,,/Art/infoFull.png��6   $��36��6I   $��4q�6U   �� 5S      -   5        $��AdrBtnLabel��5S      $��Details��6+   
�����w�$��Center=�6Y   $��16(�6{   5T      5U      �� 5V      -   5        
�������$ Close Window��5W      .��5X      �� $*��$pack://application:,,,/Art/close.png��6   $��30��6F   5Y      5Z      5\      