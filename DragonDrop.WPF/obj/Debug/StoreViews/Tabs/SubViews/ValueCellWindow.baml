   M S B A M L   `   `   `  ����I  EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews'DragonDrop.WPF.StoreViews.Tabs.SubViews  5   	   c _System.Windows.Interactivity, Version=4.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35qPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.InteractivitySystem.Windows.Interactivity 5   	   /clr-namespace:DragonDrop.WPFDragonDrop.WPF  5   	   A    ;DragonDrop.WPF.StoreViews.Tabs.SubViews.ValueCellEditWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Glocal5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews     5   	   ^iPclr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity     5   	   ,behclr-namespace:DragonDrop.WPF     5   	     � Title$  
Edit Value��5
   	    � SizeToContent$ WidthAndHeight=�6   $	��720��6;   
�����w� � WindowStartupLocation$ CenterScreen=�5   	    � 	Resources 5       �� 
TargetType �� Property �� Value��%�   '��       '�� l     �� 5   
   " ��6   .�5      ��  � �� 6+   Z� �� 5       � �� 6,   Z� 5      �� 5   
   " ��6   .�5      ��  � ��$ Center=�62   �� 5       � ��$ 18(�6)   5      5      .��5      ��  �� Orientation$ 
Horizontal=�6   .+�5   
   �� 
�������$��Value: ��6(   �� 5   
   -   5         ��Name$ entryBox��5      
���OO/�
�������$��Center=�5   %   $��20(�6@   $	��4 0q�6N   
	 � Tag$	 SelectableTxtBox��6\   ��5      �� *	ValText+
 �� Mode$
 TwoWay=� �� UpdateSourceTrigger$ PropertyChanged=��� 5   
   -   5         ��Name$ 	submitBtn��5      .��5      �� $*��$pack://application:,,,/Art/check.png��6   $��28��6B   .  (System.Windows.Interactivity.Interaction   	Behaviors 5      +   %DragonDrop.WPF.EventToCommandBehavior  5          Event$ Click��6O      PassArguments .     Command 6-   �� *SubmitCommand+6]   5"      �� 5#   
   -   5        .��5$      �� $*��$pack://application:,,,/Art/close.png��6   $��28��6B   5%      5&      5'      