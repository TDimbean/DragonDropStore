   M S B A M L   `   `   `  ����I  EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews'DragonDrop.WPF.StoreViews.Tabs.SubViews  5   	   e7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters)DragonDrop.WPF.StoreViews.Tabs.Converters  5   	   =    7DragonDrop.WPF.StoreViews.Tabs.SubViews.OrderDetailsPop   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Glocal5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews     5   	   Hconv7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters     5   	     � Title$  Order Details��5
   	    � WindowStartupLocation$ CenterScreen=�6    � WindowStyle$ 
ToolWindow=�6D    � 
ResizeMode$ NoResize=�6]    � SizeToContent$ WidthAndHeight=�5   	   ���#BFB35A � 	Resources 5      J   DDragonDrop.WPF.StoreViews.Tabs.Converters.IdToPaymentMethodConverter   IdToPayMethConvK   EDragonDrop.WPF.StoreViews.Tabs.Converters.IdToShippingMethodConverter  IdToShipMethConvH   BDragonDrop.WPF.StoreViews.Tabs.Converters.IdToOrderStatusConverter  IdToStatConv �� 
TargetType �� Property �� Value  AlternateRowBg	 �� Color  
MainColour��%�  &	        &	      &	 &     '�� 9     '�� �     &	 =    &	 _      5      6.     5      6/     5      6,   �� 5      " ��6   .�5      ��  � ��$ 18(�6-   �� 5       � ��$ Center=�66   �� 5       � ��$ Center=�6=   5      �� 5      " ��6   .�5      ��  � �� 6/   Z� �� 5       � �� 60   Z� 5      �� 5      $	 Honeydew��65   �� 5      $	 	PaleGreen��61   5      .��5       �� #��[ 6   .+�5!   
   ]� $��4q�6   .+�5"      �� $+��%pack://application:,,,/Art/orders.png��6   $��60��6C   $
��Left=�6O   $��0q�6e   �� 5#      
 �� Orientation$
 Vertical=�6   $
��Left=�60   $	��4 0q�6F   $��Center=�6S   .+�5$      �� $
 
Horizontal=�6   $��0q�66   $	��Top=�6A   .+�5%      �� $��
Order ID: ��6   $��0q�61   $��0q�6<   $��Center=�6H   $��20(�6j   �� 5&      $��0q�69   $��0q�6D   $��18(�6P   ��6   �� *	OrderId+6P   5'      �� 5(      $
 
Horizontal=�6   $��0q�66   $��Bottom=�6A   .+�5)      �� $��Status: ��6   $��0q�6/   $��0q�6:   $��Center=�6F   $��20(�6h   �� 5*      $��0q�5+      $��0q�6(   $��24(�64   ��5*      �� *OrderStatusId+ �� 	Converter# [ 5+   4   5,      5-      5.      � 50   
   #��[ 6    � ColumnDefinitions 51      �� 52      $��* �6#   �� 53      $��* �6#   .+�55      �� $
 
Horizontal=�6    � ToolTip$ Payment Method��62   .+�56      �� $.��(pack://application:,,,/Art/payMethod.png��6   $��30��6J   �� 57      ��6   �� *PaymentMethodId+# [  58      �� 5:      $
 
Horizontal=�6   $ Shipping Method��62   $��1��6L   .+�5;      �� $2��,pack://application:,,,/Art/deliveryTruck.png��6   $��30��6N   �� 5<      ��6   �� *ShippingMethodId+# [ 5=      5>      �� 5?   
   $
 
Horizontal=�6   $��2q�6.   .+�5@      �� $+��%pack://application:,,,/Art/person.png��6   $��30��6G   �� 5A      $!��Placed by Customer with ID:��6   �� 5B      $��24(�63   $
��Bold"�6A   $��Consolas)�6S   ��6   �� *
CustomerId+6S   5C      �� 5D   
   $
 Vertical=�6   #��[ 6-   .+�5E      �� $
 
Horizontal=�6   .+�5F      �� $-��'pack://application:,,,/Art/calendar.png��6   $��30��6I   $��0q�6U   $
��Left=�6a   �� 5G      $��
Placed on:��6   $��Center=�6-   $��20(�6O   �� 5H      $��Center=�6F   $��Consolas)�6h   $��24(�6~   ��6   �� *	OrderDate+ �� StringFormat$ d��6~   5I      �� 5J      $
 
Horizontal=�6   .+�5K      �� $1��+pack://application:,,,/Art/deliveryTime.png��6   $��30��6M   $��0q�6Y   $
��Left=�6e   �� 5L      $��Shipped:��6   $��Center=�6+   $��20(�6M   �� 5M      $��Center=�6I   $��Consolas)�5N      $��24(�6.   ��5M      �� *ShippingDate+$ d��5N   .   5O      5P      �� 5R   
   $
 
Horizontal=�6   
�������.+�5S      �� -   5        $	��162��5S      $ Close Window��5T      .��5U      �� $*��$pack://application:,,,/Art/close.png��6   $��30��6F   5V      �� 5W      -   5        $	��160��5W      $ Edit Product��5X      .��5Y      �� $+��%pack://application:,,,/Art/pencil.png��6   $��30��6G   5Z      5[      5\      5_      