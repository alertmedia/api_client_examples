from alertmedia.client import Client
import time
import io

def main():

    def user_examples():
        ###
        # Get Users with filters on name, phone (exact) or email
        print(alertmedia_client.user.list(name="John"))

        # Smart Search does a "contains" search across all name, email and phone fields
        print(alertmedia_client.user.list(smart_search="512"))

        ###
        # Paginated list of all users
        users_list = []
        done = False
        start = 0
        end = 4
        while not done:
            result = alertmedia_client.user.list(start=start, end=end)
            users_list.extend(result.data)
            total = result.item_range.total
            if total is None or total <= end:
                done = True
                continue
            start = end + 1
            end = end + 5

    def group_examples():
        ###
        # Get Groups
        print(alertmedia_client.group.list(start=0, end=5))
        print(alertmedia_client.group.get(group_id=4))

        # Create a user
        # At minimum, you need First Name, Last Name and either email OR mobile_phone
        new_user = {}
        new_user['first_name'] = "Guido"
        new_user['last_name'] = "Rossum"
        new_user['email'] = "guidoros5@example.com"

        result = alertmedia_client.user.create(new_user)

        if result.status_code == 400:
            # clean up this example user for future runs
            print(result.reason)
            existing_user = alertmedia_client.user.list(email=new_user['email']).data
            if len(existing_user) > 0:
                existing_user = existing_user[0]
                print("deleting existing user with email {}".format(existing_user['email']))
                print(alertmedia_client.user.delete(existing_user['id']))

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

        print(alertmedia_client.user.update(user_id=another_user['id'], params=another_user))

        ###
        # Create a new group
        result = alertmedia_client.group.create(customer=alertmedia_client.api_customer_id, name="New Group", description="my new group")
        new_group_id = result.data['id']
        print(alertmedia_client.group.add_users(new_group_id, [another_user_id]))

        # now delete the user
        print(alertmedia_client.user.delete(user_id=another_user['id']))

        # clean up the group
        print(alertmedia_client.group.delete(new_group_id))

    def notification_plan_examples():
        # list notifications
        # notifications = alertmedia_client.notification.list()

        # Plans are Notification Templates.
        # They are essentially Notification objects, with a few extra attributes
        plans = alertmedia_client.notification.list_plans(alertmedia_client.api_customer_id)

        # Get a specific plan by ID
        plan = alertmedia_client.notification.get_plan(u'< your notification template ID >')

        # A Plan can be turned into a Notification by removing a few key fields
        new_notification = plan.data

        # if the Plan has an Event, reuse it. Otherwise, create a new Event and attach it to the Notification
        if new_notification['event'] is None:
            # Get the default Event Type for your customer
            event_types = alertmedia_client.event.list_types(alertmedia_client.api_customer_id, default_only=True)
            event_type = event_types.data[0]
            event = alertmedia_client.event.create({"title": "New Event", "customer": alertmedia_client.api_customer_id, "event_type": event_type["id"]})
            new_notification['event'] = event.data

        # Send the new notification
        result = alertmedia_client.notification.create(params=new_notification)

        print(result)

    def send_notification_example():
        #send a notification over 3 channels, with a specific message, to a given user

        notification = {
            "target_formats": ["email", "app", "sms"],
            "message": "Test notification",
            "users": [123, 456, 789]
        }
        result = alertmedia_client.notification.create(params=notification)
        print(result.data)

    # Obtain the keys from your AlertMedia account
    # This can be found under Company Settings ... API
    alertmedia_client = Client(client_id="", client_key="")

    # Search and Paginate users
    #user_examples()

    # Create users, groups and add/remove users from Groups
    #group_examples()

    # List notifications & templates. Create events. Create a new notification from a template.
    #notification_examples()

    send_notification_example()


if __name__ == '__main__':
    main()