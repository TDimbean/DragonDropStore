   M S B A M L   `   `   `  ����I  EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulle7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters)DragonDrop.WPF.StoreViews.Tabs.Converters  5   	   a5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews'DragonDrop.WPF.StoreViews.Tabs.SubViews  5   	   8    2DragonDrop.WPF.StoreViews.Tabs.SubViews.ProductPop   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Hconv7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters     5   	   Glocal5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews     5   	     � Title$  Product Details��5	   	    � WindowStartupLocation$ CenterScreen=�6"    � WindowStyle$ 
ToolWindow=�6G    � 
ResizeMode$ NoResize=�6`    � SizeToContent$ WidthAndHeight=�6v   ���#8FBE00 � 	Resources 5       �� 
TargetType �� Property �� Value   AlternateRowBg	 �� Color  
MainColourE   ?DragonDrop.WPF.StoreViews.Tabs.Converters.IdToCategoryConverter  IdToCatConv��%a  '��       '�� n     &	  �     &	 �     &	     �� 5      " ��6   .�5      ��  � ��$ 18(�6-   �� 5       � ��$
 Left=�68   5      �� 5      " ��6   .�5      ��  � �� 6/   Z� �� 5       � �� 60   Z� 5      �� 5      $	 #AEE239��65   �� 5      $	 #8FBE00��61     5      6)   5      .��5      �� #��[ 6   .+�5   
   ]� $��4q�6   .+�5      �� $-��'pack://application:,,,/Art/products.png��6   $
��Left=�6�   $��0q�6�   ��6E   �� 
 �� ElementName$
 HeaderStack�� �� Path$ ActualHeight�6�   �� 5      -   5         ��Name$ HeaderStack��5       �� Orientation$ Vertical=�6.   $
��Left=�6E   $��90��6[   $��0q�6g   .+�5      �� $
��Wrap=�62   $	��260��6F   $��0q�6R    �� Padding$ 0q�6]   $��24(�6i   ��6   �� *Name+6i   �� 5      $ 
Horizontal=�6   $	��260��66   $��0q�6B   .+�5       �� $
��ID: ��6   $��0q�6+   $��0q�66   $��Center=�6B   $��16(�6d   �� 5!      $��0q�6<   $ 0q�6G   $��18(�6S   $	��230��6a   ��6!   �� *	ProductId+6a   5"      5#      5$      �� 5%   
   $ 
Horizontal=�6   #��[  6.    � ToolTip$ Category��6[   .+�5&      �� $)��#pack://application:,,,/Art/pawn.png��6   $��30��6A   $��4q�6M   �� 5'      $��Center=�6r   ��6   �� *
CategoryId+ �� 	Converter# [ ��6\   Z� 6r   5(      �� 5)   
   $ 
Horizontal=�6   .+�5*   
   �� $ 
Horizontal=�6   $	��160��6.   $ Price��6:   .+�5+      �� $*��$pack://application:,,,/Art/money.png��6   $��4q�6B   $��30��6M   �� 5,      $��Center=�6C   ��6   �� *	UnitPrice+ �� StringFormat$ c����6^   Z� 5-      �� 5/      $ 
Horizontal=�6   $ Stock��63   .+�50      �� $*��$pack://application:,,,/Art/store.png��6   $��30��6F   $��4q�6R   �� 51      $��Center=�6I   ��6   �� *Stock+��63   Z� 6I   52      54      �� 55   
   -   5        $ DetailsStack��55      $ Vertical=�6+   $y�	Collapsed=�6B    56      ��%U  '��       '�� k     '�� �     �� 58      " ��6   .�59      ��  � ��$ 24��63   �� 5:       � ��$ 4q�63   5;      �� 5<      " ��6   .�5=      ��  � ��$ Center=�6>   �� 5>       � ��$ 18(�65   5?      �� 5@      " ��6   .�5A      ��  �@��Orientation$ 
Horizontal=�68   5B      5D      .+�5E      �� #��[  6   $ Description��6F   .+�5F      �� $)��#pack://application:,,,/Art/info.png��6   $��Center=�6E   �� 5G      $
��Wrap=�69   ��6   �� *Description+69   5H      �� 5I      $ Barcode��6   .+�5J      �� $,��&pack://application:,,,/Art/barcode.png��6   �� 5K      ��6   �� *	BarCode+5L      �� 5M      #��[  6   $ Manufacturer��6F   .+�5N      �� $1��+pack://application:,,,/Art/manufacturer.png��6   �� 5O      ��6   �� *Manufacturer+5P      5Q      �� 5R   
   -   5        
�������.��5S      �� $ 
Horizontal=�6   .+�5T      �� $-��'pack://application:,,,/Art/infoFull.png��6   $��36��6I   $��4q�6U   �� 5U      -   5        $��AdrBtnLabel��5U      $��Details��6+   
�����w�$��Center=�6Y   $��16(�6{   5V      5W      �� 5X   
   $ 
Horizontal=�6   
�������.+�5Y      �� -   5        $	��160��5Y      $ Close Window��5Z      .��5[      �� $*��$pack://application:,,,/Art/close.png��6   $��30��6F   5\      �� 5]      -   5        $	��160��5]      $ Edit Customer Details��5^      .��5_      �� $+��%pack://application:,,,/Art/pencil.png��6   $��30��6G   5`      5a      5b      5d      