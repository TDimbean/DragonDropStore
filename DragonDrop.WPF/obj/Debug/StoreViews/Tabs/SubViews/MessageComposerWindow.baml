   M S B A M L   `   `   `  ����I  EDragonDrop.WPF, Version=1.0.0.0, Culture=neutral, PublicKeyToken=nulla5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews'DragonDrop.WPF.StoreViews.Tabs.SubViews  5   	   C    =DragonDrop.WPF.StoreViews.Tabs.SubViews.MessageComposerWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Glocal5clr-namespace:DragonDrop.WPF.StoreViews.Tabs.SubViews     5   	     � SizeToContent$  Height=�5   	   $	��480��6     � 
ResizeMode$ NoResize=�6,    � WindowStartupLocation$ CenterScreen=�6B    � 	Resources 5	       �� 
TargetType �� Property �� Value��%�  '��       '�� l     '��     �� 5
   
   " ��6   .�5      ��  � �� 6+   Z� �� 5       � �� 6,   Z� 5      �� 5   
   " ��6   .�5      ��  � ��$ 18(�6)   �� 5       � ��$ Center=�62   �� 5       � ��$ Center=�69   5      �� 5   
   " ��6   .�5      ��  � �� 6,   Z� �� 5       � ��$ 16(�6)   �� 5       � ��$ Center=�62   �� 5       � ��$ Center=�69   5      5      .��5      ��  �� Orientation$ Vertical=�6   .+�5   
   �� $ 
Horizontal=�6   .+�5      �� $��Contact E-mail:��6   �� 5      -   5         ��Name$ emailBox��5      	 �� 
IsReadOnly	 . 
 �� IsReadOnlyCaretVisible
 .  $	��300��5       �� MaxLines$ 1��6%   �� 5      -   5         ��Name$ editMailBtn��5      .��5       �� $+��%pack://application:,,,/Art/pencil.png��6   $��24��6G   5!      5"      ]� 5#   
   .+�5$      �� -   5        $��charCountLbl��5$      $��0/1250��6(   $��Right=�69   $��Right=�6U   �� 5%      -   5         ��Name$ msgLbl��5%      5&      �� 5'   
   -   5         �� TextWrapping$
 Wrap=�5'       �� 	MaxLength$
 1250��6H   �� 5(   
   -   5        .��5)      �� $ 
Horizontal=�6   .+�5*      �� $)��#pack://application:,,,/Art/mail.png��6   $��36��6A   �� 5+      $
��Send��6   $��Consolas)�6#   $��28(�69   5,      5-      5.      5/      