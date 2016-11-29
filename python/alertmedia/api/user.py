from alertmedia.network import Network


class User(object):
    auth = ""
    API_LIST_USERS = "/api/users?"
    API_GET_USER = "/api/users/"
    API_CREATE_USER = "/api/users"
    API_UPDATE_USER = "/api/users/"
    API_DELETE_USER = "/api/users/"
    API_VERIFY_USER = "/api/verify_user"
    API_ACTIVATE_USER = "/api/activate"
    API_LOGIN_USER = "/api/login"

    def __init__(self, auth):
        self.auth = auth
        self.net = Network(self.auth)

    def list(self, name=None, email=None, phone=None, smart_search=None, start=None, end=None):
        """
        Returns paginated list of all users
        you can pass in optional filters for name, email, and/or phone
        """
        end_point = self.API_LIST_USERS
        if name is not None:
            end_point = "{}name={}&".format(end_point, name)
        if email is not None:
            end_point = "{}email={}&".format(end_point, email)
        if phone is not None:
            end_point = "{}phone={}&".format(end_point, phone)
        if smart_search is not None:
            end_point = "{}smart_search={}&".format(end_point, smart_search)

        return self.net.get(end_point, payload=None, start=start, end=end)

    def get(self, user_id):
        url = self.API_GET_USER + str(user_id)
        payload = {}
        return self.net.get(url, payload)

    def create(self, params={}):
        return self.net.post(self.API_CREATE_USER, params)

    def update(self, user_id, params={}):
        url = self.API_GET_USER + str(user_id)
        return self.net.put(url, params)

    def delete(self, user_id):
        url = self.API_GET_USER + str(user_id)
        return self.net.delete(url)

    def login(self, username, password):
        '''
        Returns an Auth Token that can be used for further API calls peformed
        as the logged in user with the Authorization = "Token 0123456789abcdef"
        this can be used instead of the Basic Auth associated with the API keys.
        '''
        user_obj = {}
        user_obj['username'] = username
        user_obj['password'] = password
        return self.net.post(self.API_LOGIN_USER, user_obj)

    def auto_login(self):
        '''
        if you do a GET with a valid user Authorization Token,
        this return updated information if the user is still authed
        '''
        return self.net.get(self.API_LOGIN_USER, {})

    def unsubscribe(self, user_id, channels=None):
        '''
        Unsubscribe a user from specified channels ['sms', 'email', 'voice'].
        If no channels are provided, the user will be unsubscribed from all
        channels.

        **Note** Unsubscribe is only possible if your company has opted to
        "Respect User Communication Preferences" in Company settings.
        '''
        channels = channels if type(channels) == 'list' and len(channels) > 0 else None
        url = '{}{}/unsubscribe'.format(self.API_GET_USER, user_id)
        data = {}
        if channels is not None:
            data['channels'] = channels
        return self.net.post(url, data)

    def add_photo(self, user_id, files=None):
        '''
        Upload an image to set as the specified user's profile photo.
        files should be a dict of the following form:


        files = {
            'file_name': open('image.png', 'rb')
        }

        note that 'file_name' is a placeholder and that we will upload the file
        regardless of the keyname. Trying to upload more than one file will throw
        and Exception.
        '''
        if not files or len(files) > 1:
            raise 'Too many files passed to photo endpoint'
        keys = files.keys()
        if type(files[keys[0]]) != 'file':
            raise 'You must pass a file object to this endpoint'
        url = '{}{}/photo'.format(self.API_GET_USER, user_id)
        return self.net.upload(url, files=files)