   M S B A M L   `   `   `  ����I  EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulle7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters)DragonDrop.WPF.StoreViews.Tabs.Converters  5   	   a5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews'DragonDrop.WPF.StoreViews.Tabs.SubViews  5   	   6    0DragonDrop.WPF.StoreViews.Tabs.SubViews.OrderPop   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Hconv7clr-namespace:DragonDrop.WPF.StoreViews.Tabs.Converters     5   	   Glocal5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews     5   	     � Title$  Order Details��5       � WindowStartupLocation$ CenterScreen=�61    � WindowStyle$ 
ToolWindow=�6V    � 
ResizeMode$ NoResize=�6o    � SizeToContent$ WidthAndHeight=�6�   ���#ECD078 � 	Resources 5
       �� 
TargetType �� Property �� Value   AlternateRowBg	 �� Color  	MainBrushH   BDragonDrop.WPF.StoreViews.Tabs.Converters.IdToOrderStatusConverter  IdToOrdStatConvK   EDragonDrop.WPF.StoreViews.Tabs.Converters.IdToShippingMethodConverter  IdToShipMethConvJ   DDragonDrop.WPF.StoreViews.Tabs.Converters.IdToPaymentMethodConverter  IdToPayMethConv��%�  '��       '�� n     &	  �     &	 �     &	     &	 /    &	 B    �� 5      " ��6   .�5      ��  � ��$ 18(�6-   �� 5       � ��$
 Left=�68   5      �� 5      " ��6   .�5      ��  � �� 6/   Z� �� 5       � �� 60   Z� 5      �� 5      $	 #D95B43��65   �� 5      $	 #ECD078��60     5      6,     5      6/     5      6.   5      .��5      �� #��[ 6   .+�5   
   ]� $��4q�6   .+�5      �� $+��%pack://application:,,,/Art/orders.png��6   $��60��6C   $
��Left=�6O   $��4q�6e   �� 5      
 �� Orientation$
 Vertical=�6   $
��Left=�60   $��0q�6F   .+�5       �� $
 
Horizontal=�6   $	��260��66   $��0q�6B   .+�5!      �� $
��ID: ��6   $��0q�6+   $��0q�66   $��Center=�6B   $��24(�6d   �� 5"      $��0q�6:    �� Padding$ 0q�6E   $��22(�6Q   $	��230��6_   ��6!   �� *	OrderId+6_   5#      �� 5$      $	��260��6:   $��0q�6F   $ 0q�6Q   $��18(�6]   ��6   �� *CustomerInfo+6]   5%      5&      �� 5'   
   $
 
Horizontal=�6   #��[  6.   .+�5(      �� $-��'pack://application:,,,/Art/calendar.png��6   $��30��6E   $��4q�6Q   �� 5)      $��Placed:��6   $��Center=�6&   $��16(�6A   $��0q�6O   $��0q�6Z   �� 5*      $��Center=�6Y   ��6   �� *	OrderDate+ �� StringFormat$ d����6C   Z� 6Y   5+      �� 5,   
   $
 
Horizontal=�6   .+�5-      �� $1��+pack://application:,,,/Art/deliveryTime.png��6   $��4q�6I   $��30��6T   �� 5.      $��
Delivered:��6   $��Center=�6)   $��16(�6D   $��0q�6R   $��0q�6]   �� 5/      $��Center=�6F   ��6   �� *ShippingDate+$ d����6a   Z� 50      �� 51   
   $
 
Horizontal=�6   #��[  6.   .+�52      �� $.��(pack://application:,,,/Art/orderItem.png��6   $��0q�6F   $��34��6Q   �� 53      $��Status:��6   $��Center=�6&   $��16(�6A   $��0q�6O   $��0q�6Z   �� 54      $��Center=�6c   ��6   �� *OrderStatusId+ �� 	Converter# [ ��6~   Z� 55      �� 57   
   $
 
Horizontal=�6   .+�59   
   �� $
 
Horizontal=�6   $	��160��6.    � ToolTip$ Shipping Method��6:   .+�5:      �� $2��,pack://application:,,,/Art/deliveryTruck.png��6   $��0q�6J   $��30��6U   �� 5;      $��Center=�6g   ��6   �� *ShippingMethodId+# [ ��6�   Z� 5<      �� 5=   
   $
 
Horizontal=�6   $ Payment Method��6.   .+�5>      �� $.��(pack://application:,,,/Art/payMethod.png��6   $��0q�6F   $��30��6Q   �� 5?      $��Center=�6e   ��6   �� *PaymentMethodId+# [ ��6�   Z� 5@      5A      �� 5C   
   $
 
Horizontal=�6   
�������.+�5D      �� -   5        $	��160��5D      $ Close Window��5E      .��5F      �� $*��$pack://application:,,,/Art/close.png��6   $��30��6F   5G      �� 5H      -   5        $	��160��5H      $ Edit Customer Details��5I      .��5J      �� $+��%pack://application:,,,/Art/pencil.png��6   $��30��6G   5K      5L      5M      5O      