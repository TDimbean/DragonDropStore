   M S B A M L   `   `   `  ����I  EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews'DragonDrop.WPF.StoreViews.Tabs.SubViews  5   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5   	   /clr-namespace:DragonDrop.WPFDragonDrop.WPF  5   	   C    =DragonDrop.WPF.StoreViews.Tabs.SubViews.ValueCellRemoveWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Glocal5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5   	   ,behclr-namespace:DragonDrop.WPF     5   	     � Title$$  Confirm Default Value Deletion��5
   	    � SizeToContent$ WidthAndHeight=�60   
�����w� � WindowStartupLocation$ CenterScreen=�6k    � 
ResizeMode$ NoResize=�5   	    � 	Resources 5       �� 
TargetType �� Property �� Value   BoundLblStyle��%�  '��       &	  n     '��     �� 5   
   " ��6   .�5      ��  � ��
 �����6+   �� 5       � ��$ 24(�6)   5      �� 5   
   " ��6&   .�5      ��  � ��
 �����6+   �� 5       � ��$ 26(�6)   �� 5       � ��$ Consolas)�6+   5      �� 5   
   " ��6   .�5      ��  � �� 6+   Z� �� 5       � ��$
 Bold"�6+   �� 5       � ��$ Consolas)�6+   5      5      .��5      ��  �� Orientation$ Vertical=�6   .+�5   
   �� $>��8Are you sure you want to permanently remove this record?��6   �� 5   
   $ 
Horizontal=�6   $��Center=�6.   .+�5      �� $	��ID:��6   �� 5       #��[  60   ��6   �� *	ValueId+60   �� 5!      $��Value:��6   �� 5"      #��[  62   ��6   �� *	ValueName+62   5#      � 5$   
   	 � ColumnDefinitions	 5%      �� 5&      $��* �6#   �� 5'      $��* �6#   .+�5)      �� .��5*      �� $	��YES��6   .  (System.Windows.Interactivity.Interaction
   	Behaviors
 5+      +   %DragonDrop.WPF.EventToCommandBehavior  5,         Event$ Click��6P      PassArguments .     Command 61   �� *
YesCommand+6^   5.      �� 5/      -   5        $��1��5/      .��50      �� $��NO��6   51      52      53      54      