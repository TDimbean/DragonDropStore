   M S B A M L   `   `   `  ����I  EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews'DragonDrop.WPF.StoreViews.Tabs.SubViews  5   	   ?    9DragonDrop.WPF.StoreViews.Tabs.SubViews.ProductDetailsPop   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Glocal5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews     5   	     � Title$  Product Details��5   	    � WindowStartupLocation$ CenterScreen=�6!    � WindowStyle$ 
ToolWindow=�6F    � 
ResizeMode$ NoResize=�6_    � SizeToContent$ WidthAndHeight=�5	   	   ���#BFB35A � 	Resources 5       �� 
TargetType �� Property �� Value   AlternateRowBg	 �� Color  
MainColour��%D  '��       '�� n     &	  �     &	 �     �� 5      " ��6   .�5      ��  � ��$ 18(�6-   �� 5       � ��$
 Left=�68   5      �� 5      " ��6   .�5      ��  � �� 6/   Z� �� 5       � �� 60   Z� 5      �� 5      $	 #F2C45A��65   �� 5      $	 #BFB35A��61   5      .��5      �� #��[ 6   .+�5   
   ]� $��4q�6   .+�5      �� $-��'pack://application:,,,/Art/products.png��6   $��60��6E   $
��Left=�6Q   $��0q�6g   �� 5      $	��256��6   
 �� Orientation$
 Vertical=�6%   $
��Left=�6<   $��0q�6R   .+�5      �� $	��256��62    �� TextAlignment$ Center=�6>   $
��Wrap=�5       $��0q�60    �� Padding$ 0q�6;   $��24(�6G   ��5      �� *Name+5    G   �� 5!      $
 
Horizontal=�6   $	��256��66   $��0q�6B   .+�5"      �� $
��ID: ��6   $��0q�6+   $��0q�66   $��Center=�6B   $��16(�6d   �� 5#      $��0q�6<   $ 0q�6G   $��18(�6S   $	��236��6a   ��6!   �� *	ProductId+6a   5$      5%      5&      �� 5(   
   #��[  6   $
 
Horizontal=�6B    � ToolTip$ Description��6[   $	��324��6q   .+�5)      �� $)��#pack://application:,,,/Art/info.png��6   $��30��6A   $��4q�6M   $
��Left=�6X   $	��Top=�5*      �� 5+      $
��Wrap=�65   $	��260��6I    �� LineStackingStrategy$ 	MaxHeight=�6X   ��6   �� *Description+6X   5,      �� 5-   
   $
 
Horizontal=�6   $��2q�6.   .+�5.      �� $
 
Horizontal=�6   $��0��62   $ Stock��6C   .+�5/      �� $*��$pack://application:,,,/Art/store.png��6   $��30��6B   �� 50      $��Center=�6/   $ Center=�6J   $	��130��6a   ��6   �� *Stock+6a   51      �� 52      $��2q�6   $
 
Horizontal=�6$   $ Price��6>   .+�53      �� $*��$pack://application:,,,/Art/money.png��6   $��30��6F   �� 54      $��Center=�6G   $ Center=�55      $	��130��61   ��54      �� *	UnitPrice+ �� StringFormat$ C��55   1   56      57      �� 58   
   $
 
Horizontal=�6   #��[  6/   .+�59      �� $ Barcode��6   $	��162��6+   $
 
Horizontal=�67   .+�5:      �� $,��&pack://application:,,,/Art/barcode.png��6   $��34��6H   $��0q�6T   $
��Left=�6`   �� 5;      $��Center=�65   $ Center=�5<      $	��130��61   ��5;      �� *	BarCode+5<   1   5=      �� 5>      $
 
Horizontal=�6   $ Manufacturer��63   .+�5?      �� $1��+pack://application:,,,/Art/manufacturer.png��6   $��34��6J   �� 5@      $��Center=�6:   $ Center=�5A      $	��130��61   ��5@      �� *Manufacturer+5A   1   5B      5C      �� 5E   
   $
 
Horizontal=�6   
�������.+�5F      �� -   5        $	��162��5F      $ Close Window��5G      .��5H      �� $*��$pack://application:,,,/Art/close.png��6   $��30��6F   5I      �� 5J      -   5        $	��160��5J      $ Edit Product��5K      .��5L      �� $+��%pack://application:,,,/Art/pencil.png��6   $��30��6G   5M      5N      5O      5Q      