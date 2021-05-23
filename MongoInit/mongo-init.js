print('=============== START INIT SCRIPT ===============');
db = db.getSiblingDB("SocialCode");
db.createCollection("Users");
db.createCollection("Posts");
db.createCollection("Comments");

print('=============== END INIT SCRIPT ===============');
