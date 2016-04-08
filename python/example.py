from alertmedia.client import Client


def main():

    # obtain the keys from your AlertMedia account
    # This can be found under Company Settings ... API (https://dashboard.alertmedia.com/#/company/api)
    alertmedia_client = Client(client_id="", client_key="")

    ###
    # Get Users with filters on name, phone (exact) or email
    print alertmedia_client.user.list(name="John")

    # Smart Search does a "contains" search across all name, email and phone fields
    print alertmedia_client.user.list(smart_search="512")

    ###
    # Paginated list of all users
    users_list = []
    start = 0
    end = 9
    while True:
        result = alertmedia_client.user.list(start=start, end=end)
        users_list.extend(result.data)
        total = result.item_range.get('total', None)
        if total is None or total <= end:
            break
        start = end + 1
        end += 10

    ###
    # Get Groups
    print alertmedia_client.group.list(start=0, end=5)
    print alertmedia_client.group.get(group_id=1)

    # Create a user
    # At minimum, you need First Name, Last Name and either email OR mobile_phone
    new_user = {}
    new_user['first_name'] = "Guido"
    new_user['last_name'] = "Rossum"
    new_user['email'] = "guidoros5@example.com"

    result = alertmedia_client.user.create(new_user)

    if result.status_code == 400:
        # clean up this example user for future runs
        print result.reason
        existing_user = alertmedia_client.user.list(email=new_user['email']).data
        if len(existing_user) > 0:
            existing_user = existing_user[0]
            print "deleting existing user with email {}".format(existing_user['email'])
            print alertmedia_client.user.delete(existing_user['id'])

        # rerun to continue through the example normally now that we've cleaned up
        exit()

    created_user = result.data

    # Get a user, then change it.  redundantly getting user we just created for demonstration purposes
    another_user_id = created_user['id']
    another_result = alertmedia_client.user.get(user_id=another_user_id)
    another_user = another_result.data

    # change this user
    another_user['first_name'] = "Guido 1"
    another_user['last_name'] = "Rossum 1"

    print alertmedia_client.user.update(user_id=another_user['id'], params=another_user)

    ###
    # Create a new group
    result = alertmedia_client.group.create(customer=alertmedia_client.customer_id, name="New Group", description="my new group")
    new_group_id = result.data['id']
    print alertmedia_client.group.add_users(new_group_id, [another_user_id])

    # now delete the user
    print alertmedia_client.user.delete(user_id=another_user['id'])

    # clean up the group
    print alertmedia_client.group.delete(new_group_id)

if __name__ == '__main__':
    main()
