# MainGame Scene Initialize

##Priview

In order to manager the MainGame scene well, 

##Offline Mode

* Loading MainGameLoader
  * In MainGameLoader
    1. First lock the cursor
    2. Create ClientController
    3. Create InputManager
    4. Create Player
       1. Assign InputManager.instruction to Player
    5. Create Camera
       1. Assign Player's tank's tramsform to Camera as its FollowingTransform